   M S B A M L   `   `   `  ����N  JRemoteCompanyHelper, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullE'clr-namespace:RemoteCompanyHelper.ViewsRemoteCompanyHelper.Views  5   	   I)clr-namespace:RemoteCompanyHelper.HelpersRemoteCompanyHelper.Helpers  5   	   .    (RemoteCompanyHelper.Views.ScreenShowView   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   9local'clr-namespace:RemoteCompanyHelper.Views     5   	   7h)clr-namespace:RemoteCompanyHelper.Helpers     5   	   ^ ZCaliburn.Micro.Platform, Version=3.2.0.0, Culture=neutral, PublicKeyToken=8e5891231f2ed21f(calhttp://www.caliburnproject.org  5	   	     � Title$  ScreenShowView��5
   	    � WindowStyle$
 None=�6     � AllowsTransparency .  � WindowState$ 	Maximized=�5   	   ���#C000  Caliburn.Micro.Message   Attach$� [Event PreviewKeyDown] = [Action KeyDownCommand($eventArgs)]; [Event PreviewMouseWheel] = [Action MouseWheelScroll($eventArgs)]��5   	    � 	Resources 5      5   /RemoteCompanyHelper.Helpers.PercentageConverter   PercentageConverter  b2vc��%:   &	        &	        5   
   6    �� 5   
   6'   5      .��5      �  � UseLayoutRounding .  � ColumnDefinitions 5   
   �� 5      $��80 �6   �� 5      $��* �6   �� 5      $
��auto �6   �� 5      $��* �6   �� 5      $��80 �6    � RowDefinitions 5   
   �� 5      $��30 �6   �� 5      $
��auto �6   �� 5      $��* �6   .+�5   
   �� -   5        	 ��Name$	 canvas��5      $��0��6!   $��5��61   $��0��6E   $��3��6R   ._�5      �� 
 J� RenderTransformOrigin$
 0.5, 0.5&�61   ��6   �� *	CurSource+z�5       �� 5!       �� ScaleX 6%   �� *PercentSize+ �� ScaleY 6D   �� *PercentSize+5#      5$      �� 5%   
   -   5         ��Name$ PrevScr��5%      $��0��6"   $��3��6/   $��0��6@     ArrowLeftSideBtn#��[ 5&      y�6;   �� *
CanPrevScr+ �� 	Converter# [ �� 5'   
   -   5        $ NextScr��5'      $��0��6"   $��3��6/   $��4��6@     ArrowRightSideBtn#��[ 5(      y�6<   �� *
CanNextScr+# [ � 5)   
   -   5         �Name$ TopPanel��5)   ,   $��2��6   $��1��6    5*      �� 5+      $��1* �6#   �� 5,      $
��auto �6#   �� 5-      $��1* �6#    5/      �� 50      $
��0.5* �6    �� 51      $
��0.5* �6    .+�53      �� $��0��6   $��0��6%   $��3��65     BaseText#��[ 6e   $��30(�6�   ���#BBB$��Center=�54   *   ��53   I   �� *
CurPosText+54   *   �� 55      -   5        $ DecreaseImageSize��55      $��1��60   $��0��6=   $��-��6M     SemiCircleLeftBtn#��[ 56      $��Right=�6@   �� 57      -   5        $ IncreaseImageSize��57      $��1��60   $��2��6=   $��+��6M     SemiCircleRightBtn#��[ 58      $
��Left=�6A   �� 59      $��1��6   $��1��6"   ���#555$��0q�6D   ._�5:      �� #��[ 66   $	��5 0q�6X   ���#BBB$��Center=�5;   *   ���#555$��Center=�5<      $��30(�63   ��5:      �� *
SizeText+5<   3   5=      ��5@      �� 5A       �� 
TargetType" �6    �� Triggers 5B      P� 5C       P� Property �@J�IsMouseOver P� Value .  . �5D      ��  �� Property �@J�Opacity �� Value$	 0.2Q�68   6F   5E      P� 5F       �@J�IsMouseOver . . �5G      ��  �@J�Opacity$ 1Q�68   6D   5H      5J      5L      5M      5N      