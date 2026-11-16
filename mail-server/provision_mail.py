#!/usr/bin/env python3
"""
Массовое создание почтовых ящиков и алиасов сотрудников в Mailcow.

Берёт CSV со списком сотрудников, для каждого создаёт:
  * один ящик  ivan.petrov@DOMAIN  (с IMAP-доступом)
  * N алиасов  ivan.petrov.1@DOMAIN, ivan.petrov.2@DOMAIN, ...  -> в тот же ящик

Скрипт идемпотентный: уже существующие ящики/алиасы пропускаются,
поэтому его можно спокойно перезапускать (например, после добавления новых сотрудников).

Пароли новых ящиков пишутся в credentials.csv (права 600). Храните его как секрет,
после раздачи паролей сотрудникам — перенесите в менеджер паролей и удалите.

Пример:
  export MAILCOW_URL=https://mail.company.ru
  export MAILCOW_API_KEY=xxxxxxxx
  python3 provision_mail.py employees.csv --domain company.ru --dry-run
  python3 provision_mail.py employees.csv --domain company.ru
"""

import argparse
import csv
import os
import re
import secrets
import string
import sys
import time

try:
    import requests
except ImportError:
    sys.exit("Нужна библиотека requests: pip install -r requirements.txt")


# --- Транслитерация кириллицы для логинов -----------------------------------

TRANSLIT = {
    "а": "a", "б": "b", "в": "v", "г": "g", "д": "d", "е": "e", "ё": "e",
    "ж": "zh", "з": "z", "и": "i", "й": "y", "к": "k", "л": "l", "м": "m",
    "н": "n", "о": "o", "п": "p", "р": "r", "с": "s", "т": "t", "у": "u",
    "ф": "f", "х": "kh", "ц": "ts", "ч": "ch", "ш": "sh", "щ": "shch",
    "ъ": "", "ы": "y", "ь": "", "э": "e", "ю": "yu", "я": "ya",
}


def translit(text: str) -> str:
    out = "".join(TRANSLIT.get(ch, ch) for ch in text.strip().lower())
    out = re.sub(r"[^a-z0-9]+", "-", out).strip("-")
    return out


def make_local_part(first: str, last: str) -> str:
    parts = [p for p in (translit(first), translit(last)) if p]
    return ".".join(parts)


def gen_password(length: int = 16) -> str:
    alphabet = string.ascii_letters + string.digits
    while True:
        pwd = "".join(secrets.choice(alphabet) for _ in range(length))
        if (any(c.islower() for c in pwd) and any(c.isupper() for c in pwd)
                and any(c.isdigit() for c in pwd)):
            return pwd


# --- Клиент Mailcow API ------------------------------------------------------

class MailcowError(Exception):
    pass


class Mailcow:
    def __init__(self, url: str, api_key: str, timeout: int = 30):
        self.base = url.rstrip("/") + "/api/v1"
        self.s = requests.Session()
        self.s.headers.update({"X-API-Key": api_key, "Accept": "application/json"})
        self.timeout = timeout

    def _get(self, path: str):
        r = self.s.get(f"{self.base}/{path}", timeout=self.timeout)
        r.raise_for_status()
        return r.json()

    def _post(self, path: str, payload: dict):
        r = self.s.post(f"{self.base}/{path}", json=payload, timeout=self.timeout)
        r.raise_for_status()
        data = r.json()
        # Mailcow отвечает списком вида [{"type": "success"|"danger"|"error", "msg": ...}]
        items = data if isinstance(data, list) else [data]
        errors = [i for i in items if isinstance(i, dict) and i.get("type") in ("danger", "error")]
        if errors:
            raise MailcowError("; ".join(str(e.get("msg")) for e in errors))
        return data

    def existing_mailboxes(self, domain: str) -> set:
        data = self._get(f"get/mailbox/all/{domain}")
        if isinstance(data, dict):  # пустой ответ бывает {}
            data = [data] if data else []
        return {m["username"].lower() for m in data if isinstance(m, dict) and "username" in m}

    def existing_aliases(self) -> set:
        data = self._get("get/alias/all")
        if isinstance(data, dict):
            data = [data] if data else []
        return {a["address"].lower() for a in data if isinstance(a, dict) and "address" in a}

    def add_mailbox(self, local_part, domain, name, password, quota_mb):
        return self._post("add/mailbox", {
            "local_part": local_part,
            "domain": domain,
            "name": name,
            "password": password,
            "password2": password,
            "quota": str(quota_mb),
            "active": "1",
            "force_pw_update": "0",
            "tls_enforce_in": "1",
            "tls_enforce_out": "1",
        })

    def add_alias(self, address, goto):
        return self._post("add/alias", {"address": address, "goto": goto, "active": "1"})


# --- Основная логика ---------------------------------------------------------

def read_employees(path: str):
    """CSV с колонками: first_name,last_name[,login][,department]"""
    with open(path, newline="", encoding="utf-8-sig") as f:
        reader = csv.DictReader(f)
        required = {"first_name", "last_name"}
        if not required.issubset(reader.fieldnames or []):
            sys.exit(f"В CSV нужны колонки: {', '.join(sorted(required))} (есть: {reader.fieldnames})")
        for row in reader:
            row = {k: (v or "").strip() for k, v in row.items()}
            if row["first_name"] or row["last_name"]:
                yield row


def plan(employees, domain, alias_suffixes):
    """Строит список (логин, имя, алиасы), решая коллизии одинаковых ФИО."""
    used = set()
    result = []
    for e in employees:
        base = e.get("login") or make_local_part(e["first_name"], e["last_name"])
        if not base:
            print(f"! Пропуск: не удалось получить логин для {e}", file=sys.stderr)
            continue
        local = base
        n = 2
        while local in used:
            local = f"{base}{n}"
            n += 1
        used.add(local)
        name = f"{e['first_name']} {e['last_name']}".strip()
        aliases = [f"{local}.{s}@{domain}" for s in alias_suffixes]
        result.append({"local": local, "email": f"{local}@{domain}", "name": name,
                       "aliases": aliases, "department": e.get("department", "")})
    return result


def main():
    ap = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("csv", help="CSV сотрудников (first_name,last_name[,login][,department])")
    ap.add_argument("--domain", required=True, help="почтовый домен, например company.ru")
    ap.add_argument("--alias-suffixes", default="1,2,3",
                    help="суффиксы алиасов через запятую (по умолчанию 1,2,3 -> ivan.petrov.1@...)")
    ap.add_argument("--quota", type=int, default=1024, help="квота ящика в МБ (по умолчанию 1024)")
    ap.add_argument("--out", default="credentials.csv", help="куда писать пароли новых ящиков")
    ap.add_argument("--delay", type=float, default=0.2, help="пауза между запросами к API, сек")
    ap.add_argument("--dry-run", action="store_true", help="только показать, что будет создано")
    args = ap.parse_args()

    suffixes = [s.strip() for s in args.alias_suffixes.split(",") if s.strip()]
    items = plan(read_employees(args.csv), args.domain.lower(), suffixes)
    print(f"Сотрудников: {len(items)}, алиасов на человека: {len(suffixes)}")

    if args.dry_run:
        for it in items[:20]:
            print(f"  {it['email']:40} {it['name']:30} -> {', '.join(it['aliases'])}")
        if len(items) > 20:
            print(f"  ... и ещё {len(items) - 20}")
        return

    url = os.environ.get("MAILCOW_URL")
    key = os.environ.get("MAILCOW_API_KEY")
    if not url or not key:
        sys.exit("Задайте переменные окружения MAILCOW_URL и MAILCOW_API_KEY")

    mc = Mailcow(url, key)
    have_mb = mc.existing_mailboxes(args.domain)
    have_al = mc.existing_aliases()
    print(f"Уже есть ящиков: {len(have_mb)}, алиасов: {len(have_al)}")

    new_file = not os.path.exists(args.out)
    fd = os.open(args.out, os.O_WRONLY | os.O_CREAT | os.O_APPEND, 0o600)
    out = os.fdopen(fd, "a", newline="", encoding="utf-8")
    writer = csv.writer(out)
    if new_file:
        writer.writerow(["email", "password", "name", "department", "aliases"])

    created_mb = created_al = failed = 0
    try:
        for i, it in enumerate(items, 1):
            prefix = f"[{i}/{len(items)}] {it['email']}"
            if it["email"] not in have_mb:
                pwd = gen_password()
                try:
                    mc.add_mailbox(it["local"], args.domain, it["name"], pwd, args.quota)
                except (MailcowError, requests.RequestException) as e:
                    print(f"{prefix}: ОШИБКА создания ящика: {e}", file=sys.stderr)
                    failed += 1
                    continue
                writer.writerow([it["email"], pwd, it["name"], it["department"], " ".join(it["aliases"])])
                out.flush()
                created_mb += 1
                print(f"{prefix}: ящик создан")
                time.sleep(args.delay)

            for alias in it["aliases"]:
                if alias in have_al:
                    continue
                try:
                    mc.add_alias(alias, it["email"])
                    created_al += 1
                except (MailcowError, requests.RequestException) as e:
                    print(f"{prefix}: ОШИБКА алиаса {alias}: {e}", file=sys.stderr)
                    failed += 1
                time.sleep(args.delay)
    finally:
        out.close()

    print(f"\nГотово. Новых ящиков: {created_mb}, новых алиасов: {created_al}, ошибок: {failed}")
    if created_mb:
        print(f"Пароли новых ящиков записаны в {args.out} — храните его как секрет.")


if __name__ == "__main__":
    main()
