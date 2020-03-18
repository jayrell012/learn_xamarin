using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App1.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App1.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LoginPage : ContentPage
	{
		public LoginPage ()
		{
			InitializeComponent ();
			Init ();
		}

        void Init()
        {
            BackgroundColor = Constants.backgroundColor;
            Lbl_Password.TextColor = Constants.mainTextColor;
            Lbl_Username.TextColor = Constants.mainTextColor;
            ActivitySpinner.IsVisible = false;
            LoginIcon.HeightRequest = Constants.LoginIconHeight;
            LoginIcon.WidthRequest = Constants.LoginIconWidth;
            Entry_Username.BackgroundColor = Constants.entryBackgroundColor;
            Entry_Username.TextColor = Constants.entryMainTextColor;
            Entry_Password.BackgroundColor = Constants.entryBackgroundColor;
            Entry_Password.TextColor = Constants.entryMainTextColor;
            Btn_Signin.BackgroundColor = Constants.buttonBackgroundColor;
            Btn_Signin.TextColor = Constants.buttonMainTextColor;

            Entry_Username.Completed += (s, e) => Entry_Password.Focus();
            Entry_Password.Completed += (s, e) => SignInProcedure(s, e);
            Entry_Password.IsPassword = true;
        }
        void SignInProcedure(object sender, EventArgs e)
        {
            try
            {
                User user = new User(Entry_Username.Text, Entry_Password.Text);
                if (user.CheckInformation())
                {
                    DisplayAlert("Login", "Login Success", "OK");
                    ActivitySpinner.IsVisible = true;
                    Entry_Username.TextColor = Constants.entryMainTextColor;
                    Entry_Password.TextColor = Constants.entryMainTextColor;
                    Entry_Username.BackgroundColor = Constants.backgroundColor;
                    Entry_Password.BackgroundColor = Constants.backgroundColor;

                    App.UserDatabase.SaveUser(user);
                }
                else
                {
                    DisplayAlert("Login", "Login Not Correct", "OK");
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Error Message",ex.Message,"Sad");
            }
        }
    }
}