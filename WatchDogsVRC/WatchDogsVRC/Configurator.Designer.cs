namespace NotoIto.App.WatchDogsVRC
{
    partial class Configurator
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Configurator));
            this.VRNotify = new System.Windows.Forms.NotifyIcon(this.components);
            this.SuspendLayout();
            // 
            // VRNotify
            // 
            this.VRNotify.BalloonTipText = "通知は有効です";
            this.VRNotify.BalloonTipTitle = "WatchDogsVRC";
            this.VRNotify.Icon = ((System.Drawing.Icon)(resources.GetObject("VRNotify.Icon")));
            this.VRNotify.Text = "WatchDogsVRC";
            this.VRNotify.Visible = true;
            // 
            // Configurator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(225, 197);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Configurator";
            this.Text = "WatchDocsVRC 設定";
            this.Load += new System.EventHandler(this.Configurator_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon VRNotify;
    }
}

