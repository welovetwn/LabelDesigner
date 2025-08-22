namespace LabelDesigner.UI
{
    partial class ApiTestForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計產生的程式碼

        private void InitializeComponent()
        {
            this.txtUrl = new System.Windows.Forms.TextBox();
            this.btnCallApi = new System.Windows.Forms.Button();
            this.txtResult = new System.Windows.Forms.TextBox();
            this.txtInput = new System.Windows.Forms.TextBox();
            this.btnTestResolve = new System.Windows.Forms.Button();
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtUrl
            // 
            this.txtUrl.Location = new System.Drawing.Point(12, 12);
            this.txtUrl.Name = "txtUrl";
            this.txtUrl.Size = new System.Drawing.Size(400, 23);
            this.txtUrl.TabIndex = 0;
            this.txtUrl.Text = "https://mocki.io/v1/0a7cfae3-09a0-4ad6-9408-123456789abc"; // 測試用 API
            // 
            // btnCallApi
            // 
            this.btnCallApi.Location = new System.Drawing.Point(420, 12);
            this.btnCallApi.Name = "btnCallApi";
            this.btnCallApi.Size = new System.Drawing.Size(100, 23);
            this.btnCallApi.TabIndex = 1;
            this.btnCallApi.Text = "呼叫 API";
            this.btnCallApi.UseVisualStyleBackColor = true;
            this.btnCallApi.Click += new System.EventHandler(this.btnCallApi_Click);
            // 
            // txtResult
            // 
            this.txtResult.Location = new System.Drawing.Point(12, 50);
            this.txtResult.Multiline = true;
            this.txtResult.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtResult.Name = "txtResult";
            this.txtResult.Size = new System.Drawing.Size(508, 100);
            this.txtResult.TabIndex = 2;
            // 
            // txtInput
            // 
            this.txtInput.Location = new System.Drawing.Point(12, 170);
            this.txtInput.Name = "txtInput";
            this.txtInput.Size = new System.Drawing.Size(508, 23);
            this.txtInput.TabIndex = 3;
            this.txtInput.Text = "Hello, {{Name}}! City = {{City}}";
            // 
            // btnTestResolve
            // 
            this.btnTestResolve.Location = new System.Drawing.Point(12, 200);
            this.btnTestResolve.Name = "btnTestResolve";
            this.btnTestResolve.Size = new System.Drawing.Size(100, 23);
            this.btnTestResolve.TabIndex = 4;
            this.btnTestResolve.Text = "測試套用";
            this.btnTestResolve.UseVisualStyleBackColor = true;
            this.btnTestResolve.Click += new System.EventHandler(this.btnTestResolve_Click);
            // 
            // txtOutput
            // 
            this.txtOutput.Location = new System.Drawing.Point(12, 230);
            this.txtOutput.Multiline = true;
            this.txtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.Size = new System.Drawing.Size(508, 100);
            this.txtOutput.TabIndex = 5;
            // 
            // ApiTestForm
            // 
            this.ClientSize = new System.Drawing.Size(534, 341);
            this.Controls.Add(this.txtOutput);
            this.Controls.Add(this.btnTestResolve);
            this.Controls.Add(this.txtInput);
            this.Controls.Add(this.txtResult);
            this.Controls.Add(this.btnCallApi);
            this.Controls.Add(this.txtUrl);
            this.Name = "ApiTestForm";
            this.Text = "API 測試工具";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox txtUrl;
        private System.Windows.Forms.Button btnCallApi;
        private System.Windows.Forms.TextBox txtResult;
        private System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.Button btnTestResolve;
        private System.Windows.Forms.TextBox txtOutput;
    }
}
