   M S B A M L   `   `   `  ����T  PMahApps.Metro, Version=1.1.2.0, Culture=neutral, PublicKeyToken=f4fb5a3c4d1e5b4fV;clr-namespace:MahApps.Metro.Controls;assembly=MahApps.MetroMahApps.Metro.Controls  5   	   @ <DCPal, Version=0.0.0.2, Culture=neutral, PublicKeyToken=nullclr-namespace:DCPalDCPal 5   	      DCPal.LoginForm   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   PControls;clr-namespace:MahApps.Metro.Controls;assembly=MahApps.Metro     5   	   X TXceed.Wpf.Toolkit, Version=2.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  5   	   localclr-namespace:DCPal  5   	   (   "MahApps.Metro.Controls.MetroWindow    TitleForeground
  �����   EnableDWMDropShadow .  � Title$ Login to DCPal��5   	    � Icon$ Resources/dcpal_icon.ico��6       	TitleCaps .   � 
ResizeMode$ NoResize=�6R   $��478.431��6h   $��300.49��6y   ���	#FF343232.��5      � .+�5   
   �� -   5         ��Name$ label��5      $��DCPal��6   $
��Left=�6/   $��	95,10,0,0q�6J   $	��Top=�6]   $��32(�6u   $��90��6�   ���	#FFC33D3D
�������6�   �� 5   
   -   5        $ label1��5      $��	Username:��6    $
��Left=�64   $��26��6O   $��
10,167,0,0q�6[   $	��Top=�6o   $��67��6�   
�������6�   �� 5   
   -   5        $ label2��5      $ ��Sign in to QuickCode Forge��6    $
��Left=�6E   $��28��6`   $��
10,134,0,0q�6l   $	��Top=�6�   $	��157��6�   
�������6�   �� 5   
   -   5        $ label3��5      $��	Password:��6    $
��Left=�64   $��26��6O   $��
10,208,0,0q�6[   $	��Top=�6o   $��67��6�   
�������6�   �� 5   
   -   5         ��Name$ txtUserName��5      $
��Left=�6'   $��23��6B   $��
77,168,0,0q�6N    �� TextWrapping$
 Wrap=�6b   $�� ��6v   $	��Top=�6~   $	��188��6�   ���	#FF494444���	#FF131111
�������	 �� MaxLines$	 1��6�   G� 5   
   -   5        
 G�Name$
 txtPassword��5      $
��Left=�6+   $��
77,208,0,0q�6F   $	��Top=�6Z   $	��188��6r   ���	#FF403C3C���	#FF131313
�������6�   �� 5   
   -   5         ��Name$ 	btnSignIn��5       �� 	IsDefault . $��Sign In��65   $
��Left=�6G   $��110,290,0,0q�6b   $	��Top=�6w   $��75��6�   ���	#FF464040
�������6�   �� 5   
   -   5        $ label2_Copy��5      $��Login��6%   $
��Left=�65   $��45��6P   $��
110,75,0,0q�6\   $	��Top=�6p   $��80��6�   
�������$��21.333(�6�   $
��Bold"�6�   �� 5   
   -	   5         ��Name$ 	textBlock��5      $
��Left=�6(   $��
10,256,0,0q�6C   $
��Wrap=�6W   $	��Top=�6k   �� 5      -
   5        $&�� http://quickcodeforge.com/signup?�5      .�6w   &$Don't have an account? Sign up here!5      5      %  Xceed.Wpf.Toolkit.BusyIndicator  5   
   -   5          Name$ logInBusyIndicator��5         IsBusy .     BusyContent$ Connecting...��6H   5      5      5!      