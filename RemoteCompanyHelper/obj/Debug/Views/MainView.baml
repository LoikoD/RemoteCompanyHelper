   M S B A M L   `   `   `  ����N  JRemoteCompanyHelper, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullE'clr-namespace:RemoteCompanyHelper.ViewsRemoteCompanyHelper.Views  5   	   O,clr-namespace:RemoteCompanyHelper.ViewModelsRemoteCompanyHelper.ViewModels  5   	   c _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35qPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.InteractivitySystem.Windows.Interactivity 5	   	   (    "RemoteCompanyHelper.Views.MainView   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   9local'clr-namespace:RemoteCompanyHelper.Views     5   	   <src,clr-namespace:RemoteCompanyHelper.ViewModels     5   	   ^iPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity     5	   	   d `SciChart.Wpf.UI.Transitionz, Version=3.1.251.0, Culture=neutral, PublicKeyToken=8eea68d53aa73a835tz,http://schemas.abtsoftware.co.uk/transitionz  5
   	   ^ ZCaliburn.Micro.Platform, Version=3.2.0.0, Culture=neutral, PublicKeyToken=8e5891231f2ed21f(calhttp://www.caliburnproject.org  5   	   -   5          �Name$  MainViewName��5   	    � Title$ MainView��5   	   $	��720��6   $
��1280��6'    � WindowStartupLocation$ CenterScreen=�64   $	��600��5   	   $	��800��6    � 	Resources 5         b2vc  tb_mouse_over �� ImageSource  
font_style �� 
TargetType �� Property �� Value  tt_style  tb_style �� 
TargetType	 ��Name
 J� SnapsToDevicePixels �� Triggers P� Property P� Value �� 
TargetName��%f  &	        &	      &	 L     &	 \    &	 �    7  7  7  7  �� 5   
   6'   �� 5   
   $! /Assets/tgBtn_MouseOver.png��5      �� 5   
   " ��6#   .�5      ��  � ��$$ Fonts/Roboto-Light.ttf #Roboto)�5      �� 5       � ��$ Center=�5      �� 5        � ��$
 Left=�5!      �� 5#       � ��
 �����5$      �� 5&       � ��$ 20(�5'      5)      �� 5-   
   " _�6   .�5/      ��  �@_�	Placement$ Right=�6*   �� 50       � ��$ 40��6'   �� 51       � ��$	 110��6&   �� 52       � ��$ 20,0,0,0q�6'   �� 53       � �� �#4A56686+   �� 54       � ��
 ���� 6,   �� 55       � ��
 �����6+   �� 56       � ��$$ Fonts/Roboto-Light.ttf #Roboto)�6+   �� 57       � ��$ 16(�6)   �� 58       �@_�PlacementRectangle$ 60,0,0,0�63   �� 59       �@_�HorizontalOffset$ 15��61   �� 5:       �@_�VerticalOffset$ 6��6/   �� 5;       �@_�HasDropShadow . 6.   �� 5<       � ��$ Center=�6;   5=      �� 5@   
   " d�6!   .�5B      ��  � �� 5D      �� 5F      " d�6&   .R�5H      �� $	 border��6"   #��z��5I   "   #��z��5J   "   
 . 5M       5O      P� 5Q       �@J�IsMouseOver . . �5S   "   �� $ border��5T   )   8    � ��5U   )   �� 5W   "   $ border��5X   )   8   � ��5Y   )   5[      P� 5]       �@d�	IsChecked$
 trueY�6;   . �5_   "   �� $ border��5`   )   8   � ��5a   )   �� 5c   "   $ border��5d   )   8   � ��5e   )   5g      5k      5o      5q      5s      .��5u      � .+�5w   
   9�   BaseExpander#���  6   � 5z   
   .+�5~      � -   5         �Name$ img_bg��5~   (   $��65, 0, 0, 0q�6     DarkBaseBrush#��[ 6a    J� Opacity 68   ��  �� Path$ MainPageOpacity� � ColumnDefinitions 5      �� 5�      $��20 �6+   �� 5�      �� 5�      $��20 �6+    � RowDefinitions 5�      �� 5�      $��20 �6$   �� 5�      $��* �6$   �� 5�      $��20 �6$   .+�5�      �� -   5         ��Name$ 
ActiveItem��5�   >   $��1��6!   $��1��61   6>   �� 5�      $��0��6   $��3��6&   $��0��67   $��3��6G   ���#5000y�5�   ,   �� *	IsLoading+ �� 	Converter# [  ._�5�      �� $��Center=�6   $��Center=�6:     MediumBaseBrush#��[ 5�      $��0 0 65 0q�6K   $	��100��5�      $	��100��6)    �� CornerRadius$ 20��66    J� Effect 5�      3  -System.Windows.Media.Effects.DropShadowEffect  5�         Color$
 #111��6/      	Direction$	 225Q�6<      ShadowDepth$ 7Q�6L   ._�5�      �� $��Загрузка...��6$     BaseText#��[ 67   $��Center=�5�   $   $��Center=�6A   5�      5�      5�      � 5�      -   5        $ nav_pnl��5�      $
��Left=�5�      $��65��5�      ���#2E333A  Caliburn.Micro.Message   Attach$1 +[Event MouseLeave] = [Action MenuToggleOff]��5�      .+�5�      �� -   5         ��Name$ st_pnl��5�      .+�5�      � $��90��6   .+�5�      �� $��1��6%   $��0��5�   !   $��
73,-20,0,0q�5�   !   $��M E N U��5�   !   
�������$��22(�5�   !   
������ $
��Left=�5�   !   $��Center=�5�   !   #��[ 5�   !    5�        5�   "      
BlurRadius$ 10Q�64   $ 1Q�5�   0   $	 -90Q�5�   0   $ White��5�   0   5�      d� 5�      -   5         d�Name$ Tg_Btn��5�   '   $��0��5�   #   $��Center=�5�   #   $
��Left=�5�   #   $��
18,-20,0,0q�5�   #   $��30��5�   #   $��30��5�   #   $��0q�5�   #   #��[ 5�   #   $0 *[Event MouseEnter] = [Action MenuToggleOn]��5�   '    d� 	IsChecked 5�   '   �� $ MenuChecked���5�      �� 5�   "   $! ../Assets/tgBtn_default.png��6-     j� Stretch$
  None=�5�   )   ! � Triggers! 5�      :� 5�   "   " :� RoutedEvent$" ToggleButton.Unchecked��6/   .[�5�   "   �� .��5�   &   �� -   5        # ��Name$# HideStackPanel��5�   1   .��5�   *   W� $ �� 
TargetName$$ nav_pnl��5�   -   % �� TargetProperty$% Width�5�   -   & g� 	BeginTime$& 0:0:0X�5�   -   ' W� From$	' 230X�5�   -   	( W� To$( 65X�68   ) g� Duration$) 0:0:0.2D�5�   -   5�   +   5�   '   5�   #   5�      :� 5�   "   $" ToggleButton.Checked��6/   .[�5�   &   �� .��5�   *   �� -   5        $# ShowStackPanel��5�   5   .��5�   *   W� $$ nav_pnl��5�   -   $% Width�5�   -   $& 0:0:0X�5�   -   $' 65X�5�   -   $	( 230X�67   $) 0:0:0.3D�5�   -   5�   +   5  '   5  '   5  #   5
     5     �� 5     -	   5        * ��Name$* 	MenuItems��5     
������ 
������ $��Disabled=�5     ��5     �� 5     .R�5     �� + �� Orientation$+ 
Horizontal=�6)   $	��230��5  )   $
��Left=�65   $��0,0,0,0q�5  )   $: 4[Event MouseLeftButtonUp] = [Action MenuItemCommand]��5  )   .+�5  "   �� $
��None=�5  $   $��Center=�63   $��12,0,0,0q�5  $   ��5  (   �� $ IconPath�5  $   �� 5  "   $��25,0,0,0q�5   (   #��[ 5!  (   ��5  ,   �� $
 Name�5!  (   , � ToolTip, 5"  "   _� 5#  &   #��[ 5%  .   ��5$  .   �� $
 Name�y�5&  .   �� *DataContext.ToolTipVisibility+- �� Source- %  System.Windows.Markup.Reference  *	MenuItems+# [  5)     5,     ��53     �� 55     " ��6$   .�56  "   ��  � ��$ Center=�6F   �� 57  "    � �� 59  &   �� 5;  *   " ��6:   .R�5=  .   �� $	 Bd��65   $��50��5>  1   #��z��5?  1   #��z��5@  1   #��z��5A  1   . �� Padding#. z��5B  1   
 . ._�5E  2   �� #��z��6C   #
 z
 5F  ?   #��z��5G  ?   5I  /    5K  .   ]� 5M  2   / ]� 
Conditions/ 5O  6   �� 5Q  :   0 �� Property0 �@J�IsMouseOver1 �� Value1 . 6[   ./�5U  6   �� $ Bd��5V  9    � �� �#404D5B5W  9   �� 5Y  6   $ Bd��5Z  9    � �� �#404D5B5[  9   5]  3   5a  +   5e  #   5g     5k     5m     5p     5r     5t     5u     