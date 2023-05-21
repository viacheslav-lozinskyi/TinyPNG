using Microsoft.VisualStudio.Shell;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace resource.tool
{
    [Guid("27C32964-F45A-40F6-902C-6EB4F5E7048E")]
    internal class VSOptions : DialogPage
    {
        private Panel m_Window = null;

        public static string GetAPIKey()
        {
            return Environment.GetEnvironmentVariable("TINYPNG_APIKEY", EnvironmentVariableTarget.User);
        }

        protected override IWin32Window Window
        {
            get
            {
                if (m_Window == null)
                {
                    {
                        m_Window = new Panel();
                        m_Window.BackColor = Color.Transparent;
                        m_Window.SizeChanged += __SizeChanged;
                    }
                    {
                        var a_Context1 = new PictureBox();
                        var a_Context2 = Convert.FromBase64String("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");
                        var a_Context3 = new MemoryStream(a_Context2, 0, a_Context2.Length);
                        {
                            a_Context1.Image = Image.FromStream(a_Context3, true);
                            a_Context1.SizeMode = PictureBoxSizeMode.StretchImage;
                            a_Context1.SetBounds(0, 0, 256, 256);
                            m_Window.Controls.Add(a_Context1);
                        }
                    }
                    {
                        var a_Context1 = new Panel();
                        {
                            a_Context1.Name = "LINE1";
                            a_Context1.SetBounds(0, 256, 100000, 2);
                            a_Context1.BackColor = SystemColors.GrayText;
                            m_Window.Controls.Add(a_Context1);
                        }
                    }
                    {
                        var a_Context1 = new Label();
                        {
                            a_Context1.Name = "DESCRIPTION";
                            a_Context1.BackColor = Color.Transparent;
                            a_Context1.ForeColor = SystemColors.GrayText;
                            a_Context1.AutoSize = false;
                            a_Context1.Text = "This service uses smart lossy compression techniques to reduce the file size of your WEBP, JPEG and PNG files. By selectively decreasing the number of colors in the image, fewer bytes are required to store the data. The effect is nearly invisible but it makes a very large difference in file size!";
                            a_Context1.SetBounds(280, 150, 700, 100);
                            m_Window.Controls.Add(a_Context1);
                        }
                    }
                    {
                        var a_Context1 = new Label();
                        {
                            a_Context1.Name = "TITLE";
                            a_Context1.BackColor = Color.Transparent;
                            a_Context1.Font = new Font("Arial", 48, FontStyle.Bold | FontStyle.Italic);
                            a_Context1.Text = "TinyPNG";
                            a_Context1.SetBounds(260, 60, 700, 80);
                            m_Window.Controls.Add(a_Context1);
                        }
                    }
                    {
                        var a_Context1 = new Label();
                        {
                            a_Context1.Text = "API Key:";
                            a_Context1.TextAlign = ContentAlignment.MiddleRight;
                            a_Context1.SetBounds(0, 280, 170, 24);
                            m_Window.Controls.Add(a_Context1);
                        }
                    }
                    {
                        var a_Context1 = new TextBox();
                        {
                            a_Context1.Name = "APIKEY";
                            a_Context1.BackColor = Color.Pink;
                            a_Context1.SetBounds(180, 280, 500, 24);
                            a_Context1.KeyDown += __KeyDown;
                            a_Context1.TextChanged += __TextChanged;
                            a_Context1.Text = GetAPIKey();
                            m_Window.Controls.Add(a_Context1);
                        }
                        {
                            a_Context1.Focus();
                        }
                    }
                    {
                        var a_Context1 = new LinkLabel();
                        {
                            a_Context1.Name = "LINK";
                            a_Context1.Text = "Get API Key";
                            a_Context1.SetBounds(180, 310, 500, 24);
                            a_Context1.LinkClicked += __LinkClicked;
                            m_Window.Controls.Add(a_Context1);
                        }
                        {
                            a_Context1.Focus();
                        }
                    }
                    {
                        var a_Context1 = new Label();
                        {
                            a_Context1.Text = "Report file:";
                            a_Context1.TextAlign = ContentAlignment.MiddleRight;
                            a_Context1.SetBounds(0, 346, 170, 24);
                            m_Window.Controls.Add(a_Context1);
                        }
                    }
                    {
                        var a_Context1 = new TextBox();
                        {
                            a_Context1.Name = "FILE";
                            a_Context1.Text = "$(SolutionFolder)\\tinypng.json";
                            a_Context1.ReadOnly = true;
                            a_Context1.SetBounds(180, 346, 500, 24);
                            m_Window.Controls.Add(a_Context1);
                        }
                    }
                    {
                        var a_Context1 = new Panel();
                        {
                            a_Context1.Name = "LINE2";
                            a_Context1.SetBounds(0, 390, 100000, 2);
                            a_Context1.BackColor = SystemColors.GrayText;
                            m_Window.Controls.Add(a_Context1);
                        }
                    }
                    {
                        var a_Context1 = new Label();
                        {
                            a_Context1.Name = "FREE_USAGE";
                            a_Context1.Text = "This extension is free, but the service which does this optimization isn't free.\r\nIt provides compression for 500 free images in month.\r\nOther conversions will be necessary to purchase.";
                            a_Context1.AutoSize = false;
                            a_Context1.SetBounds(0, 410, 500, 100);
                            m_Window.Controls.Add(a_Context1);
                        }
                    }
                }
                return m_Window;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            if (m_Window != null)
            {
                var a_Context = m_Window.Controls.Find("APIKEY", true)[0] as TextBox;
                if (a_Context != null)
                {
                    a_Context.Text = GetAPIKey();
                }
            }
        }

        protected override void OnApply(DialogPage.PageApplyEventArgs e)
        {
            if ((m_Window != null) && (e.ApplyBehavior == ApplyKind.Apply))
            {
                var a_Context = m_Window.Controls.Find("APIKEY", true)[0] as TextBox;
                if (a_Context != null)
                {
                    Environment.SetEnvironmentVariable("TinyPNG_APIKEY", a_Context.Text.Trim(), EnvironmentVariableTarget.User);
                }
            }
        }

        private void __LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://tinypng.com/developers?utm_source=Visual%20Studio&utm_medium=organic&utm_campaign=metaoutput&utm_content=www.metaoutput.net");
        }

        private void __SizeChanged(object sender, EventArgs e)
        {
            __SizeUpdate(sender, "TITLE");
            __SizeUpdate(sender, "DESCRIPTION");
            __SizeUpdate(sender, "APIKEY");
            __SizeUpdate(sender, "LINK");
            __SizeUpdate(sender, "APIKEY");
            __SizeUpdate(sender, "FILE");
            __SizeUpdate(sender, "LINE1");
            __SizeUpdate(sender, "LINE2");
            __SizeUpdate(sender, "FREE_USAGE");
        }

        private void __KeyDown(object sender, KeyEventArgs e)
        {
            var a_Context = sender as TextBox;
            if ((a_Context != null) && ((e.KeyValue == 'V') && e.Control) || ((e.KeyCode == Keys.Insert) && e.Shift))
            {
                a_Context.SelectedText = Clipboard.GetDataObject().GetData("System.String").ToString();
                e.Handled = true;
            }
        }

        private void __TextChanged(object sender, EventArgs e)
        {
            var a_Context = sender as TextBox;
            if (a_Context != null)
            {
                a_Context.BackColor = a_Context.Text?.Length < 20 ? Color.Pink : SystemColors.Window;
            }
        }

        private void __SizeUpdate(object sender, string name)
        {
            var a_Context = sender as Panel;
            if (a_Context != null)
            {
                var a_Context1 = a_Context.Controls.Find(name, true)[0];
                if (a_Context1 != null)
                {
                    a_Context1.SetBounds(a_Context1.Left, a_Context1.Top, a_Context.Width - a_Context1.Left, a_Context1.Height);
                }
            }
        }
    }
}
