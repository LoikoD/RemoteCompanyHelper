#!/usr/bin/env python3
"""
Сбор писем подтверждения регистрации из ящиков сотрудников по IMAP.

Проходит по ящикам из credentials.csv (результат provision_mail.py), ищет письма
от нужного отправителя и вытаскивает из них ссылки и/или коды подтверждения.
Для каждого письма определяется, на какой алиас оно пришло (ivan.petrov.2@...),
то есть к какому из аккаунтов сотрудника оно относится.

Результат пишется в CSV: mailbox, alias, date, subject, links, codes.

Два способа входа:
  1) по паролям каждого ящика из credentials.csv (по умолчанию);
  2) через мастер-пользователя Dovecot (не нужно хранить пароли сотрудников):
     --master-user NAME, пароль в переменной окружения IMAP_MASTER_PASS.
     В Mailcow логин выглядит как  user@domain*NAME  (см. README).

Примеры:
  python3 fetch_confirmations.py --host mail.company.ru --from noreply@service.example \
      --link-regex 'https://service\\.example/confirm[^\\s"<>]+' --since-days 2

  python3 fetch_confirmations.py --host mail.company.ru --from service.example \
      --code-regex '\\b\\d{6}\\b' --only-unseen --mark-seen
"""

import argparse
import csv
import datetime as dt
import email
import email.policy
import imaplib
import os
import re
import sys
from concurrent.futures import ThreadPoolExecutor, as_completed
from email.utils import getaddresses, parsedate_to_datetime

DEFAULT_LINK_RE = r'https?://[^\s"<>\']+'


def load_accounts(path):
    with open(path, newline="", encoding="utf-8") as f:
        for row in csv.DictReader(f):
            aliases = [a.lower() for a in (row.get("aliases") or "").split() if a]
            yield {"email": row["email"].lower(), "password": row.get("password", ""), "aliases": aliases}


def message_text(msg) -> str:
    parts = []
    for part in msg.walk() if msg.is_multipart() else [msg]:
        if part.get_content_maintype() != "text":
            continue
        try:
            parts.append(part.get_content())
        except Exception:
            payload = part.get_payload(decode=True) or b""
            parts.append(payload.decode(part.get_content_charset() or "utf-8", "replace"))
    return "\n".join(parts)


def detect_alias(msg, account) -> str:
    """На какой адрес сотрудника пришло письмо (алиас или основной ящик)."""
    known = set(account["aliases"]) | {account["email"]}
    headers = []
    for h in ("To", "Cc", "Delivered-To", "X-Original-To"):
        headers += msg.get_all(h, [])
    for _, addr in getaddresses(headers):
        if addr.lower() in known:
            return addr.lower()
    return account["email"]


def process_account(account, args):
    login = account["email"]
    password = account["password"]
    if args.master_user:
        login = f"{account['email']}*{args.master_user}"
        password = os.environ["IMAP_MASTER_PASS"]

    results = []
    conn = imaplib.IMAP4_SSL(args.host, args.port)
    try:
        conn.login(login, password)
        conn.select("INBOX", readonly=not args.mark_seen)

        since = (dt.date.today() - dt.timedelta(days=args.since_days)).strftime("%d-%b-%Y")
        criteria = ["SINCE", since, "FROM", f'"{args.sender}"']
        if args.only_unseen:
            criteria.insert(0, "UNSEEN")
        typ, data = conn.search(None, *criteria)
        if typ != "OK":
            return results

        for num in data[0].split():
            typ, msg_data = conn.fetch(num, "(BODY.PEEK[])")
            if typ != "OK" or not msg_data or not msg_data[0]:
                continue
            msg = email.message_from_bytes(msg_data[0][1], policy=email.policy.default)
            text = message_text(msg)

            links = sorted(set(re.findall(args.link_regex, text))) if args.link_regex else []
            codes = sorted(set(re.findall(args.code_regex, text))) if args.code_regex else []
            try:
                date = parsedate_to_datetime(msg["Date"]).isoformat() if msg["Date"] else ""
            except (TypeError, ValueError):
                date = msg["Date"] or ""

            results.append({
                "mailbox": account["email"],
                "alias": detect_alias(msg, account),
                "date": date,
                "subject": str(msg["Subject"] or ""),
                "links": " ".join(links),
                "codes": " ".join(codes),
            })
            if args.mark_seen:
                conn.store(num, "+FLAGS", "\\Seen")
    finally:
        try:
            conn.logout()
        except Exception:
            pass
    return results


def main():
    ap = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("--host", required=True, help="IMAP-сервер, например mail.company.ru")
    ap.add_argument("--port", type=int, default=993)
    ap.add_argument("--accounts", default="credentials.csv", help="CSV из provision_mail.py")
    ap.add_argument("--from", dest="sender", required=True,
                    help="адрес или домен отправителя писем сервиса (подстрока для IMAP FROM)")
    ap.add_argument("--since-days", type=int, default=3, help="искать письма за последние N дней")
    ap.add_argument("--link-regex", default=DEFAULT_LINK_RE,
                    help="регулярка для ссылок подтверждения ('' — не искать ссылки)")
    ap.add_argument("--code-regex", default="", help=r"регулярка для кодов, например '\b\d{6}\b'")
    ap.add_argument("--only-unseen", action="store_true", help="только непрочитанные")
    ap.add_argument("--mark-seen", action="store_true", help="помечать найденные письма прочитанными")
    ap.add_argument("--master-user", help="имя мастер-пользователя Dovecot (пароль в IMAP_MASTER_PASS)")
    ap.add_argument("--workers", type=int, default=4, help="параллельных IMAP-подключений")
    ap.add_argument("--out", default="confirmations.csv")
    args = ap.parse_args()

    if args.master_user and "IMAP_MASTER_PASS" not in os.environ:
        sys.exit("Для --master-user задайте переменную окружения IMAP_MASTER_PASS")

    accounts = list(load_accounts(args.accounts))
    print(f"Ящиков для проверки: {len(accounts)}")

    rows, errors = [], 0
    with ThreadPoolExecutor(max_workers=args.workers) as pool:
        futures = {pool.submit(process_account, a, args): a for a in accounts}
        for fut in as_completed(futures):
            acc = futures[fut]
            try:
                found = fut.result()
                rows += found
                if found:
                    print(f"{acc['email']}: писем {len(found)}")
            except Exception as e:
                errors += 1
                print(f"{acc['email']}: ОШИБКА {e}", file=sys.stderr)

    rows.sort(key=lambda r: (r["mailbox"], r["alias"], r["date"]))
    with open(args.out, "w", newline="", encoding="utf-8") as f:
        w = csv.DictWriter(f, fieldnames=["mailbox", "alias", "date", "subject", "links", "codes"])
        w.writeheader()
        w.writerows(rows)

    print(f"\nНайдено писем: {len(rows)}, ошибок подключения: {errors}. Результат: {args.out}")


if __name__ == "__main__":
    main()
