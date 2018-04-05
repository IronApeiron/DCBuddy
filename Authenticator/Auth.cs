using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// These classes will be responsible for the verification of a user's credentials. It will also be responsible to monitor
/// potential changes made to the user's credentials, and in the event that the credentials change, the app is closed. 
/// </summary>
namespace Authenticator
{
    /// <summary>
    /// Models a User of the app.
    /// </summary>
    public class User
    {
        /// <summary>
        /// The alias of the user.
        /// </summary>
        public String LoginID { get; }

        /// <summary>
        /// Models the permissions of the user.
        /// </summary>
        public int SecurityLevel { get; set; }

        /// <summary>
        /// The status of the user.
        /// </summary>
        public String AccountStatus { get; set; }

        /// <summary>
        /// The password digest of the user.
        /// </summary>
        public String PasswordDigest { get; set; }

        /// <summary>
        /// Instantiates a User of the app.
        /// </summary>
        public User(String loginID, String passwordDigest, int securityLevel, String accountStatus)
        {
            this.SecurityLevel = securityLevel;
            this.PasswordDigest = passwordDigest;
            this.LoginID = loginID;
            this.AccountStatus = accountStatus;
        }

        NpgsqlConnection conn;

        /// <summary>
        /// Returns an NpgsqlConnection to the database if the user's credentials are still valid.
        /// </summary>
        public NpgsqlConnection Verify()
        {
            // Removing db functionality without crashing the app

            try
            {
                conn.Close();
            }catch(Exception ex) {
                Debug.WriteLine(ex.ToString());
            }
            // Grab the credentials stored in the App.config file (more secure than hardcoding)
            ConnectionStringSettings credentialSettings = ConfigurationManager.ConnectionStrings["herokuCredentials"];

            // Establish a connection to the Heroku database (postgreSQL)
            // conn = new NpgsqlConnection(credentialSettings.ConnectionString);

            try {
                conn.Open();
            }catch(Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            // Check to see if the current password and login ID haven't changed since logging in.
            NpgsqlCommand cmd = new NpgsqlCommand("select login_id, password_digest, security_level, email_confirmed, status from users where login_id = " + "@loginID" + "", conn);
            cmd.Parameters.AddWithValue("@loginID", LoginID); 

            // Read the entry from the query
            NpgsqlDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                // Check if the password is correct
                if (dr[1].ToString() == PasswordDigest)
                {
                    if (!Boolean.Parse(dr[3].ToString()))
                    {
                        conn.Close();
                        throw new Exception("You must verify your account before using DCPal.");
                    }

                    // Check to see if the user is banned or not
                    if (Int32.Parse(dr[2].ToString()) == 4)
                    {
                        conn.Close();
                        throw new Exception("Your account has been disabled. Please contact an admin for more details.");
                    }

                    // Get the external IP of the local machine and see if it's banned or not
                    string externalIP = new WebClient().DownloadString("http://bot.whatismyipaddress.com");

                    conn.Close();
                    conn = new NpgsqlConnection(credentialSettings.ConnectionString);
                    conn.Open();

                    cmd = new NpgsqlCommand("select count(*) from IPBan WHERE ip = " + "@externalIP" + "", conn);
                    cmd.Parameters.AddWithValue("@externalIP", externalIP);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());

                    if (count >= 1)
                    {
                        conn.Close();
                        // Banned...
                        throw new Exception("Your IP is blocked.");
                    }
       
                    return conn;
                }
            }
            else
            {
                conn.Close();
                throw new Exception("Password has changed.");
            }
            conn.Close();
            throw new Exception("Password has changed!");
            return null;
        }

        /// <summary>
        /// Equivalent for checking if the security level of the user is 0.
        /// </summary>
        /// <returns></returns>
        public bool IsAdmin()
        {
            return SecurityLevel == 0;
        }

        /// <summary>
        /// Equivalent for checking if the security level of the user is 1.
        /// </summary>
        /// <returns></returns>
        public bool IsDirector()
        {
            return SecurityLevel == 1;
        }

        /// <summary>
        /// Equivalent for checking if the security level of the user is 2.
        /// </summary>
        /// <returns></returns>
        public bool IsVIP()
        {
            return SecurityLevel == 2;
        }

        /// <summary>
        /// Equivalent for checking if the security level of the user is 3.
        /// </summary>
        /// <returns></returns>
        public bool IsOrdinaryMember()
        {
            return SecurityLevel == 3;
        }
    }

    /// <summary>
    /// Handles the verification of a local user.
    /// </summary>
    public class LoginManager
    {
        /// <summary>
        /// An instance of the local user.Timer
        /// </summary>
        public static User LocalUser { get; set; }

        /// <summary>
        /// Sets a localUser's credentials to be constantly monitored for changes. If a change is made,
        /// then the app will be forced to stop.
        /// </summary>
        /// <param name="localUser">The local user for the app.</param>
        public static void Monitor(User localUser)
        {
            // Set the local user for the current session.
            LocalUser = localUser;

            // UPDATE: Remove database functionality (commented out below)

            /*
            // Verify that the user's credentials haven't changed.
            NpgsqlConnection conn = localUser.Verify();

            // Make this person show up as online
            NpgsqlCommand onlineCmd = new NpgsqlCommand("insert into DCPal_OnlineUsers VALUES(" + "@loginID" + ");", conn);
            onlineCmd.Parameters.AddWithValue("@loginID", localUser.LoginID);
            onlineCmd.ExecuteNonQuery(); */
        }

        /// <summary>
        /// Authenticates a User given a username, password or a password digest, indicated by the optional isDigest flag.
        /// </summary>
        /// <param name="loginID">The alias of the user.</param>
        /// <param name="password">The plaintext password or the password digest of the user.</param>
        /// <param name="isDigest">Indicates if the passed in password is a passwordDigest or a plaintext password.</param>
        /// <returns>If successful, then a new User instance is made containing the information about the user.</returns>
        public static User Authenticate(String loginID, String password, bool isDigest=false)
        {
            // Load the database connection details securely
            ConnectionStringSettings credentialSettings = ConfigurationManager.ConnectionStrings["herokuCredentials"];

            // Open the connection
            NpgsqlConnection conn = new NpgsqlConnection(credentialSettings.ConnectionString);
            conn.Open();

            // Find the user in the database
            NpgsqlCommand cmd = new NpgsqlCommand("select login_id, password_digest, security_level, email_confirmed, status from users where login_id = " + "@loginID" + "", conn);
            cmd.Parameters.AddWithValue("@loginID", loginID);

            // Read the entries
            NpgsqlDataReader dr = cmd.ExecuteReader();

            // Get the external IP of the local machine for logging
            string externalIP = new WebClient().DownloadString("http://bot.whatismyipaddress.com");
            NpgsqlDateTime currentDate = NpgsqlDateTime.Now;

            if (dr.Read())
            {
                // Check if the password is correct and check to see if the user has verified their email address
                if (BCrypt.Net.BCrypt.Verify(password, dr[1].ToString()))
                {
                    if (!Boolean.Parse(dr[3].ToString()))
                    {
                        conn.Close();

                        // Log the failure
                        conn = new NpgsqlConnection(credentialSettings.ConnectionString);
                        conn.Open();

                        cmd = new NpgsqlCommand("INSERT INTO system_access_details VALUES(default, " + "@externalIP" + ", 'none', 'Failure - Reason: Not verified', 'Log In to DCPal', '" + currentDate.ToString() + "', '"
                               + currentDate.ToString() + "', " + "@loginID" + ")", conn);
                        cmd.Parameters.AddWithValue("@externalIP", externalIP);
                        cmd.Parameters.AddWithValue("@loginID", loginID);


                        cmd.ExecuteNonQuery();
                        conn.Close();
                        throw new Exception("You must verify your account before using DCPal.");
                    }

                    // Check to see if the user is banned or not
                    if(Int32.Parse(dr[2].ToString()) == 4)
                    {
                        conn.Close();

                        // Log the failure
                        conn = new NpgsqlConnection(credentialSettings.ConnectionString);
                        conn.Open();

                        cmd = new NpgsqlCommand("INSERT INTO system_access_details VALUES(default, " + "@externalIP" + ", 'none', 'Failure - Reason: User account is banned.', 'Log In to DCPal', '" + currentDate.ToString() + "', '"
                               + currentDate.ToString() + "', " + "@loginID" + ")", conn);

                        cmd.Parameters.AddWithValue("@externalIP", externalIP);
                        cmd.Parameters.AddWithValue("@loginID", loginID);

                        cmd.ExecuteNonQuery();
                        conn.Close();
                        throw new Exception("Your account has been disabled. Please contact an admin for more details.");
                    }

                    User u = new User(loginID, dr[1].ToString(), Int32.Parse(dr[2].ToString()), dr[4].ToString());
                    conn.Close();

                    conn = new NpgsqlConnection(credentialSettings.ConnectionString);
                    conn.Open();

                    // Check for IP ban
                    cmd = new NpgsqlCommand("select * from IPBan WHERE ip = " + "@externalIP" + "", conn);
                    cmd.Parameters.AddWithValue("@externalIP", externalIP);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());

                    if (count == 1)
                    {
                        conn.Close();
                        // Banned...
                        // Log the failure
                        conn = new NpgsqlConnection(credentialSettings.ConnectionString);
                        conn.Open();
                        cmd = new NpgsqlCommand("INSERT INTO system_access_details VALUES(default, " + "@externalIP" + ", 'none', 'Failure - Reason: User account is IP banned.', 'Log In to DCPal', '" + currentDate.ToString() + "', '"
                               + currentDate.ToString() + "', " + "@loginID" + ")", conn);

                        cmd.Parameters.AddWithValue("@externalIP", externalIP);
                        cmd.Parameters.AddWithValue("@loginID", loginID);


                        cmd.ExecuteNonQuery();
                        conn.Close();
                        throw new Exception("Your IP is blocked.");
                    }


                    // Make a system login entry
                    currentDate = NpgsqlDateTime.Now;
                    cmd = new NpgsqlCommand("INSERT INTO system_access_details VALUES(default, " + "@externalIP" + ", 'none', 'Success', 'Log In to DCPal', '" + currentDate.ToString() + "', '"
                           + currentDate.ToString() + "', " + "@loginID" + ")", conn);

                    cmd.Parameters.AddWithValue("@externalIP", externalIP);
                    cmd.Parameters.AddWithValue("@loginID", loginID);

                    cmd.ExecuteNonQuery();

                    conn.Close();
                    // Return a new instance of the user
                    return u; 
                }
            }
            else
            {
                conn.Close();

                // Log the failure
                conn = new NpgsqlConnection(credentialSettings.ConnectionString);
                conn.Open();

                cmd = new NpgsqlCommand("INSERT INTO system_access_details VALUES(default, " + "@externalIP" + ", 'none', 'Failure - Reason: Incorrect password.', 'Log In to DCPal', '" + currentDate.ToString() + "', '"
                       + currentDate.ToString() + "', " + "@loginID" + ")", conn);

                cmd.Parameters.AddWithValue("@externalIP", externalIP);
                cmd.Parameters.AddWithValue("@loginID", loginID);


                cmd.ExecuteNonQuery();
                conn.Close();

                throw new Exception("Incorrect username and/or password!");
            }
            conn.Close();
            throw new Exception("Incorrect username and/or password!");
        }


    }
}
