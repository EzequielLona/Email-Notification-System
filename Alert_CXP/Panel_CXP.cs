using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System;
using System.Runtime.InteropServices;
using System.DirectoryServices.AccountManagement;
using System.Security.Principal;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using Microsoft.Win32;
using System.Net.NetworkInformation;

namespace Alert_CXP
{
    public partial class Panel_CXP : Form
    {
        List<string> ArchivoAdjuntos = new List<string>();
        int vCXP_timer00 = 0;
        int vCXP_timer01 = 0;
        int Cvisible00 = 0;
        int Enable_Time00 = 0;
        int sendmail00 = 0;
        int sendmail01 = 0;

        int send_opening_software00 = 1;


        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hwnd, int wmsg, int wparam, int lparam);

        public string GetLoggedInUserName()
        {
            // Open the "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Authentication\LogonUI" registry key
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Authentication\LogonUI");

            if (key != null)
            {
                // Get the "LastLoggedOnUser" value
                object value = key.GetValue("LastLoggedOnUser");

                if (value != null)
                {
                    // Return the username
                    return value.ToString();
                }
            }

            return null;
        }


        public string GetIPAddress()
        { 
    // Crea una variable para almacenar las direcciones IP como cadena
    string ipAddress = "";

        // Obtiene todas las interfaces de red del sistema
        NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();

    // Itera por cada interfaz de red
    foreach (NetworkInterface ni in interfaces)
    {
        // Filtra las interfaces que no están conectadas o son virtuales
        if (ni.OperationalStatus != OperationalStatus.Up || ni.Description.Contains("Virtual") || ni.Description.Contains("VMware") || ni.Description.Contains("VirtualBox"))
        {
            continue;
        }

        // Filtra las interfaces que no tienen dirección IP
        if (ni.GetIPProperties().UnicastAddresses.Count == 0)
        {
            continue;
        }

                // Itera por cada dirección IP de la interfaz de red
                foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
        {
            // Filtra las direcciones IP IPv6
            if (ip.Address.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
            {
                continue;
            }

                    // Filtra la dirección IP de localhost o 127.0.0.1
                    if (ip.Address.ToString() == "127.0.0.1" || ip.Address.ToString() == "::1")
                    {
                        continue;
                    }
                    // Concatena la dirección IP de la interfaz de red en la cadena
                    ipAddress += $"{ip.Address.ToString()}, ";
        }
}

// Elimina la coma y el espacio final de la cadena
if (ipAddress.EndsWith(", "))
{
    ipAddress = ipAddress.Substring(0, ipAddress.Length - 2);
}

return ipAddress;
        }


        public Panel_CXP()
        {
            InitializeComponent();
        }
        private void Panel_CXP_Load(object sender, EventArgs e)
        {
            var Time = ConfigurationSettings.AppSettings["Time_min"];
            var Cvisible01 = ConfigurationSettings.AppSettings["Cvisible"];
            Cvisible00 = Int32.Parse(Cvisible01);

            if (Cvisible00 == 0)
            {
                this.Visible = false;
                this.WindowState = FormWindowState.Minimized;
                this.Hide();
                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
                this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                this.BackColor = System.Drawing.SystemColors.Control;
                this.ClientSize = new System.Drawing.Size(585, 492);
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                this.MaximizeBox = false;
                this.MinimizeBox = false;
                this.Name = "Panel_CXP";
                this.Opacity = 0D;
                this.ShowIcon = false;
                this.ShowInTaskbar = false;
                this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
                this.Text = "Form1";
                this.TransparencyKey = System.Drawing.Color.Transparent;
                this.Load += new System.EventHandler(this.Panel_CXP_Load);
                this.ResumeLayout(false);
                this.PerformLayout();
                txtAdjunto.Text = ConfigurationSettings.AppSettings["Cvisible"];
            }

            vCXP_timer01 = Int32.Parse(Time) * 600;
            CXP_Timer00.Start();


        }
        private void CXP_Timer00_Tick(object sender, EventArgs e)
        {
            string Enable_Time01 = ConfigurationSettings.AppSettings["Enable_Time"];
            Enable_Time00 = Int32.Parse(Enable_Time01);
            txtTiempo.Text = Convert.ToString(vCXP_timer01);
            if (Enable_Time00 == 1)
            {

                vCXP_timer00 = vCXP_timer00 + 1;
                txtBxCXP_Timer00.Text = Convert.ToString(vCXP_timer00);

                if (vCXP_timer00 == vCXP_timer01)
                {
                    var application_01 = ConfigurationSettings.AppSettings["application_01"];

                    Process[] miProceso = Process.GetProcessesByName(application_01);
                    if (miProceso.Length > 0)
                    {
                        string strHostName = string.Empty;
                        strHostName = Dns.GetHostName();

                        string application = ConfigurationSettings.AppSettings["application_01"];

                        string userName2 = GetLoggedInUserName();

                        
                        string App_status = ConfigurationSettings.AppSettings["App_status"];
                        string App_run = ConfigurationSettings.AppSettings["App_run"];
                        string App_name = ConfigurationSettings.AppSettings["App_name"];
                        string App_IP = ConfigurationSettings.AppSettings["App_IP"];
                        string App_Hostname = ConfigurationSettings.AppSettings["App_Hostname"];
                        string App_username = ConfigurationSettings.AppSettings["App_username"];

                        string Mail_data00 = ConfigurationSettings.AppSettings["Mail_data00"];
                        string Mail_data01 = ConfigurationSettings.AppSettings["Mail_data01"];
                        String Mail_data02 = ConfigurationSettings.AppSettings["Mail_data02"];
                        String Mail_data03 = ConfigurationSettings.AppSettings["Mail_data03"];
                        // IPAddress[] hostIPs = Dns.GetHostAddresses(strHostName);
                        string hostIPs = GetIPAddress();

                        for (int i = 0; i < hostIPs.Length; i++)
                        {
                            txtMensaje.Text = Mail_data00 + String.Format(Environment.NewLine) + String.Format(Environment.NewLine) + App_status + App_run + String.Format(Environment.NewLine) + App_name + application + String.Format(Environment.NewLine) + App_IP + hostIPs +  String.Format(Environment.NewLine) + App_Hostname + strHostName + String.Format(Environment.NewLine) + App_username + userName2 + String.Format(Environment.NewLine) + String.Format(Environment.NewLine) + Mail_data01 + String.Format(Environment.NewLine) + String.Format(Environment.NewLine) + Mail_data02 + String.Format(Environment.NewLine) + String.Format(Environment.NewLine) + Mail_data03;
                        }

                        try
                        {
                            //CONFIGURAR VALORES
                            var Port = ConfigurationSettings.AppSettings["Mail_Port"];

                            string Host = ConfigurationSettings.AppSettings["Mail_smtp"];
                            int Puerto = Int32.Parse(Port);
                            string Usuario = ConfigurationSettings.AppSettings["Mail_User"];
                            string Clave = ConfigurationSettings.AppSettings["Mail_Pass"];
                            txtPara.Text = ConfigurationSettings.AppSettings["Mail_send"];
                            txtCopia.Text = ConfigurationSettings.AppSettings["Mail_cc"];
                            txtAsunto.Text = ConfigurationSettings.AppSettings["Mail_subject"];

                            //PROPORCIONAMOS AUTENTICACION DE 
                            SmtpClient smtp = new SmtpClient(Host, Puerto);
                            MailMessage msg = new MailMessage();


                            //CREAMOS EL CONTENIDO DEL CORREO
                            string[] Destinatario = txtPara.Text.Split(',');
                            string[] DestinatarioCopia = txtCopia.Text.Split(',');
                            string[] DestinatarioCopiaOculta = txtCopiaOculta.Text.Split(',');

                            string Mail_Profile = ConfigurationSettings.AppSettings["Mail_Profile"];
                            msg.From = new MailAddress(Usuario, Mail_Profile);
                            foreach (string correo in Destinatario) if (correo != "") msg.To.Add(new MailAddress(correo));
                            foreach (string correo in DestinatarioCopia) if (correo != "") msg.CC.Add(new MailAddress(correo));
                            foreach (string correo in DestinatarioCopiaOculta) if (correo != "") msg.Bcc.Add(new MailAddress(correo));
                            foreach (string adjunto in ArchivoAdjuntos) if (adjunto != "") msg.Attachments.Add(new Attachment(adjunto));
                            msg.Subject = txtAsunto.Text;
                            msg.IsBodyHtml = false;
                            msg.Body = txtMensaje.Text;


                            //ENVIAMOS EL CORREO
                            smtp.Credentials = new NetworkCredential(Usuario, Clave);
                            smtp.EnableSsl = true;
                            smtp.Send(msg);                     

                            //Limpiar de manera rapida
                            foreach (Control c in this.Controls)
                            {
                                if (c is TextBox)
                                {
                                    c.Text = "";
                                }
                            }
                            //Enfoco en el primer TextBox
                            this.txtPara.Focus();
                            this.txtMensajeError.BackColor = System.Drawing.Color.WhiteSmoke;

                            txtMensajeError.Text = "mail sent successfully";
                        }
                        catch
                        {
                            txtMensajeError.Text = "Error when sending file configuration review";
                            this.txtMensajeError.BackColor = System.Drawing.Color.Red;
                        }

                    }

                    vCXP_timer00 = 0;
                }
            }

            var send_opening_software01 = ConfigurationSettings.AppSettings["send_opening_software"];
            send_opening_software00 = Int32.Parse(send_opening_software01);

            if (send_opening_software00 == 1)
            {
                txtBxCXP_Timer00.Text = Convert.ToString(vCXP_timer00);

                var application_01 = ConfigurationSettings.AppSettings["application_01"];

                Process[] miProceso = Process.GetProcessesByName(application_01);
                if (miProceso.Length > 0)
                {
                    CXP_Timer01.Start();
                    CXP_Timer02.Stop();
                    sendmail01 = 0;
                }
                else
                {
                    sendmail00 = 0;
                }
            }
        }//
        private void btnAdjunto_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                foreach (string adjunto in ofd.SafeFileNames)
                {
                    txtAdjunto.Text = txtAdjunto.Text + adjunto + " | ";
                }
                foreach (string adjunto in ofd.FileNames)
                {
                    ArchivoAdjuntos.Add(adjunto);
                }
            }
        }
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            ArchivoAdjuntos = new List<string>();
            txtAdjunto.Text = "";
        }
        private void BarraTitulo_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void iconcerrar_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void iconrestaurar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            iconrestaurar.Visible = false;
            iconmaximizar.Visible = true;
        }

        private void iconminimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnEnviar_Click(object sender, EventArgs e)
        {

        }

        private void CXP_Timer01_Tick(object sender, EventArgs e)
        {
            sendmail00 = sendmail00 + 1;
            txtBxCXP_Timer01.Text = Convert.ToString(sendmail00);
            if (sendmail00 == 20)
            {
                string strHostName = string.Empty;
                strHostName = Dns.GetHostName();

                string application = ConfigurationSettings.AppSettings["application_01"];

                string userName2 = GetLoggedInUserName();

                string App_status = ConfigurationSettings.AppSettings["App_status"];
                string App_run = ConfigurationSettings.AppSettings["App_run"];
                string App_name = ConfigurationSettings.AppSettings["App_name"];
                string App_IP = ConfigurationSettings.AppSettings["App_IP"];
                string App_Hostname = ConfigurationSettings.AppSettings["App_Hostname"];
                string App_username = ConfigurationSettings.AppSettings["App_username"];

                string Mail_data00 = ConfigurationSettings.AppSettings["Mail_data00"];
                string Mail_data01 = ConfigurationSettings.AppSettings["Mail_data01"];
                String Mail_data02 = ConfigurationSettings.AppSettings["Mail_data02"];
                String Mail_data03 = ConfigurationSettings.AppSettings["Mail_data03"];


                // IPAddress[] hostIPs = Dns.GetHostAddresses(strHostName);
                string hostIPs = GetIPAddress();

                for (int i = 0; i < hostIPs.Length; i++)
                {
                    txtMensaje.Text = Mail_data00 + String.Format(Environment.NewLine) + String.Format(Environment.NewLine) + App_status + App_run + String.Format(Environment.NewLine) + App_name + application + String.Format(Environment.NewLine) + App_IP + hostIPs + String.Format(Environment.NewLine) + App_Hostname + strHostName + String.Format(Environment.NewLine) + App_username + userName2 + String.Format(Environment.NewLine) + String.Format(Environment.NewLine) + Mail_data01 + String.Format(Environment.NewLine) + String.Format(Environment.NewLine) + Mail_data02 + String.Format(Environment.NewLine) + String.Format(Environment.NewLine) + Mail_data03;
                }

                try
                {
                    //CONFIGURAR VALORES
                    var Port = ConfigurationSettings.AppSettings["Mail_Port"];

                    string Host = ConfigurationSettings.AppSettings["Mail_smtp"];
                    int Puerto = Int32.Parse(Port);
                    string Usuario = ConfigurationSettings.AppSettings["Mail_User"];
                    string Clave = ConfigurationSettings.AppSettings["Mail_Pass"];
                    txtPara.Text = ConfigurationSettings.AppSettings["Mail_send"];
                    txtCopia.Text = ConfigurationSettings.AppSettings["Mail_cc"];
                    txtAsunto.Text = ConfigurationSettings.AppSettings["Mail_subject"];

                    //PROPORCIONAMOS AUTENTICACION DE GMAIL
                    SmtpClient smtp = new SmtpClient(Host, Puerto);
                    MailMessage msg = new MailMessage();


                    //CREAMOS EL CONTENIDO DEL CORREO
                    string[] Destinatario = txtPara.Text.Split(',');
                    string[] DestinatarioCopia = txtCopia.Text.Split(',');
                    string[] DestinatarioCopiaOculta = txtCopiaOculta.Text.Split(',');

                    string Mail_Profile = ConfigurationSettings.AppSettings["Mail_Profile"];
                    msg.From = new MailAddress(Usuario, Mail_Profile);
                    foreach (string correo in Destinatario) if (correo != "") msg.To.Add(new MailAddress(correo));
                    foreach (string correo in DestinatarioCopia) if (correo != "") msg.CC.Add(new MailAddress(correo));
                    foreach (string correo in DestinatarioCopiaOculta) if (correo != "") msg.Bcc.Add(new MailAddress(correo));
                    foreach (string adjunto in ArchivoAdjuntos) if (adjunto != "") msg.Attachments.Add(new Attachment(adjunto));
                    msg.Subject = txtAsunto.Text;
                    msg.IsBodyHtml = false;
                    msg.Body = txtMensaje.Text;


                    //ENVIAMOS EL CORREO
                    smtp.Credentials = new NetworkCredential(Usuario, Clave);
                    smtp.EnableSsl = true;
                    smtp.Send(msg);

                    //Limpiar de manera rapida
                    foreach (Control c in this.Controls)
                    {
                        if (c is TextBox)
                        {
                            c.Text = "";
                        }
                    }
                    //Enfoco en el primer TextBox
                    this.txtPara.Focus();
                    this.txtMensajeError.BackColor = System.Drawing.Color.WhiteSmoke;

                    txtMensajeError.Text = "mail sent successfully";
                }
                catch
                {
                    txtMensajeError.Text = "Error when sending file configuration review";
                    this.txtMensajeError.BackColor = System.Drawing.Color.Red;
                }

            }//a

            if (sendmail00 == 600)
            {
                sendmail00 = 31;
            }

            var send_opening_software01 = ConfigurationSettings.AppSettings["send_opening_software"];
            send_opening_software00 = Int32.Parse(send_opening_software01);

            if (send_opening_software00 == 1)
            {

                txtBxCXP_Timer00.Text = Convert.ToString(vCXP_timer00);

                var application_01 = ConfigurationSettings.AppSettings["application_01"];

                Process[] miProceso = Process.GetProcessesByName(application_01);
                if (miProceso.Length > 0)
                { }
                else
                {
                    CXP_Timer02.Start();
                }
            }
        }
        private void CXP_Timer02_Tick(object sender, EventArgs e)
        {
            sendmail01 = sendmail01 + 1;
            txtBxCXP_Timer02.Text = Convert.ToString(sendmail01);
            if (sendmail01 == 20)
            {
                string strHostName = string.Empty;
                strHostName = Dns.GetHostName();

                string application = ConfigurationSettings.AppSettings["application_01"];

                string userName2 = GetLoggedInUserName();

                string App_status = ConfigurationSettings.AppSettings["App_status"];
                string App_stop = ConfigurationSettings.AppSettings["App_stop"];
                string App_name = ConfigurationSettings.AppSettings["App_name"];
                string App_IP = ConfigurationSettings.AppSettings["App_IP"];
                string App_Hostname = ConfigurationSettings.AppSettings["App_Hostname"];
                string App_username = ConfigurationSettings.AppSettings["App_username"];

                string Mail_data00 = ConfigurationSettings.AppSettings["Mail_data00"];
                string Mail_data01 = ConfigurationSettings.AppSettings["Mail_data01"];
                String Mail_data02 = ConfigurationSettings.AppSettings["Mail_data02"];
                String Mail_data03 = ConfigurationSettings.AppSettings["Mail_data03"];


                // IPAddress[] hostIPs = Dns.GetHostAddresses(strHostName);
                string hostIPs = GetIPAddress();

                for (int i = 0; i < hostIPs.Length; i++)
                {
                    txtMensaje.Text = Mail_data00 + String.Format(Environment.NewLine) + String.Format(Environment.NewLine) + App_status + App_stop + String.Format(Environment.NewLine) + App_name + application + String.Format(Environment.NewLine) + App_IP + hostIPs + String.Format(Environment.NewLine) + App_Hostname + strHostName + String.Format(Environment.NewLine) + App_username + userName2 + String.Format(Environment.NewLine) + String.Format(Environment.NewLine) + Mail_data01 + String.Format(Environment.NewLine) + String.Format(Environment.NewLine) + Mail_data02 + String.Format(Environment.NewLine) + String.Format(Environment.NewLine) + Mail_data03;
                }

                try
                {
                    //CONFIGURAR VALORES
                    var Port = ConfigurationSettings.AppSettings["Mail_Port"];

                    string Host = ConfigurationSettings.AppSettings["Mail_smtp"];
                    int Puerto = Int32.Parse(Port);
                    string Usuario = ConfigurationSettings.AppSettings["Mail_User"];
                    string Clave = ConfigurationSettings.AppSettings["Mail_Pass"];
                    txtPara.Text = ConfigurationSettings.AppSettings["Mail_send"];
                    txtCopia.Text = ConfigurationSettings.AppSettings["Mail_cc"];
                    txtAsunto.Text = ConfigurationSettings.AppSettings["Mail_subject"];

                    //PROPORCIONAMOS AUTENTICACION DE GMAIL
                    SmtpClient smtp = new SmtpClient(Host, Puerto);
                    MailMessage msg = new MailMessage();


                    //CREAMOS EL CONTENIDO DEL CORREO
                    string[] Destinatario = txtPara.Text.Split(',');
                    string[] DestinatarioCopia = txtCopia.Text.Split(',');
                    string[] DestinatarioCopiaOculta = txtCopiaOculta.Text.Split(',');

                    string Mail_Profile = ConfigurationSettings.AppSettings["Mail_Profile"];
                    msg.From = new MailAddress(Usuario, Mail_Profile);
                    foreach (string correo in Destinatario) if (correo != "") msg.To.Add(new MailAddress(correo));
                    foreach (string correo in DestinatarioCopia) if (correo != "") msg.CC.Add(new MailAddress(correo));
                    foreach (string correo in DestinatarioCopiaOculta) if (correo != "") msg.Bcc.Add(new MailAddress(correo));
                    foreach (string adjunto in ArchivoAdjuntos) if (adjunto != "") msg.Attachments.Add(new Attachment(adjunto));
                    msg.Subject = txtAsunto.Text;
                    msg.IsBodyHtml = false;
                    msg.Body = txtMensaje.Text;


                    //ENVIAMOS EL CORREO
                    smtp.Credentials = new NetworkCredential(Usuario, Clave);
                    smtp.EnableSsl = true;
                    smtp.Send(msg);

                    //Limpiar de manera rapida
                    foreach (Control c in this.Controls)
                    {
                        if (c is TextBox)
                        {
                            c.Text = "";
                        }
                    }
                    //Enfoco en el primer TextBox
                    this.txtPara.Focus();
                    this.txtMensajeError.BackColor = System.Drawing.Color.WhiteSmoke;

                    txtMensajeError.Text = "mail sent successfully";
                }
                catch
                {
                    txtMensajeError.Text = "Error when sending file configuration review";
                    this.txtMensajeError.BackColor = System.Drawing.Color.Red;
                }

            }

            if (sendmail01 == 600)
            {
                sendmail01 = 31;
            }
        }
    }
}