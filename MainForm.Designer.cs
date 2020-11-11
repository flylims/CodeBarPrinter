using System.ComponentModel;

namespace LIMSCodeBarPrinter
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblOrder = new System.Windows.Forms.Label();
            this.tbOrder = new System.Windows.Forms.TextBox();
            this.btnPrint = new System.Windows.Forms.Button();
            this.tbLog = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lblOrder
            // 
            this.lblOrder.AutoSize = true;
            this.lblOrder.Location = new System.Drawing.Point(12, 9);
            this.lblOrder.Name = "lblOrder";
            this.lblOrder.Size = new System.Drawing.Size(126, 15);
            this.lblOrder.TabIndex = 0;
            this.lblOrder.Text = "Введите номер заказа";
            // 
            // tbOrder
            // 
            this.tbOrder.Anchor =
                ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top |
                                                        System.Windows.Forms.AnchorStyles.Left) |
                                                       System.Windows.Forms.AnchorStyles.Right)));
            this.tbOrder.Font = new System.Drawing.Font("Segoe UI Semibold", 14.25F, System.Drawing.FontStyle.Bold,
                System.Drawing.GraphicsUnit.Point, ((byte) (204)));
            this.tbOrder.Location = new System.Drawing.Point(12, 27);
            this.tbOrder.Name = "tbOrder";
            this.tbOrder.Size = new System.Drawing.Size(350, 33);
            this.tbOrder.TabIndex = 1;
            this.tbOrder.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnPrint
            // 
            this.btnPrint.Anchor =
                ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top |
                                                        System.Windows.Forms.AnchorStyles.Left) |
                                                       System.Windows.Forms.AnchorStyles.Right)));
            this.btnPrint.Location = new System.Drawing.Point(12, 66);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(350, 33);
            this.btnPrint.TabIndex = 2;
            this.btnPrint.Text = "Печать";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.BtnPrintClick);
            // 
            // tbLog
            // 
            this.tbLog.Anchor =
                ((System.Windows.Forms.AnchorStyles) ((((System.Windows.Forms.AnchorStyles.Top |
                                                         System.Windows.Forms.AnchorStyles.Bottom) |
                                                        System.Windows.Forms.AnchorStyles.Left) |
                                                       System.Windows.Forms.AnchorStyles.Right)));
            this.tbLog.Location = new System.Drawing.Point(12, 105);
            this.tbLog.Multiline = true;
            this.tbLog.Name = "tbLog";
            this.tbLog.ReadOnly = true;
            this.tbLog.Size = new System.Drawing.Size(350, 113);
            this.tbLog.TabIndex = 3;
            this.tbLog.WordWrap = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(374, 230);
            this.Controls.Add(this.tbLog);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.tbOrder);
            this.Controls.Add(this.lblOrder);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "FlyLIMS :: Печать ШК";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblOrder;
        private System.Windows.Forms.TextBox tbLog;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.TextBox tbOrder;
    }
}