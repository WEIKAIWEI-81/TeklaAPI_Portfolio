namespace TrueDreams_BIM_TeklaAPI
{
    partial class Form2
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
            this.button1 = new System.Windows.Forms.Button();
            this.ColumnsProfileTextBox = new System.Windows.Forms.TextBox();
            this.profileCatalog1 = new Tekla.Structures.Dialog.UIControls.ProfileCatalog();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.structuresExtender.SetAttributeName(this.button1, null);
            this.structuresExtender.SetAttributeTypeName(this.button1, null);
            this.structuresExtender.SetBindPropertyName(this.button1, null);
            this.button1.Location = new System.Drawing.Point(12, 47);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(186, 48);
            this.button1.TabIndex = 0;
            this.button1.Text = "開始計算數量";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // ColumnsProfileTextBox
            // 
            this.structuresExtender.SetAttributeName(this.ColumnsProfileTextBox, null);
            this.structuresExtender.SetAttributeTypeName(this.ColumnsProfileTextBox, null);
            this.structuresExtender.SetBindPropertyName(this.ColumnsProfileTextBox, null);
            this.ColumnsProfileTextBox.Location = new System.Drawing.Point(12, 16);
            this.ColumnsProfileTextBox.Name = "ColumnsProfileTextBox";
            this.ColumnsProfileTextBox.Size = new System.Drawing.Size(118, 22);
            this.ColumnsProfileTextBox.TabIndex = 3;
            this.ColumnsProfileTextBox.Text = "請選擇斷面";
            // 
            // profileCatalog1
            // 
            this.structuresExtender.SetAttributeName(this.profileCatalog1, null);
            this.structuresExtender.SetAttributeTypeName(this.profileCatalog1, null);
            this.profileCatalog1.BackColor = System.Drawing.Color.Transparent;
            this.structuresExtender.SetBindPropertyName(this.profileCatalog1, null);
            this.profileCatalog1.ButtonText = "albl_Select__";
            this.profileCatalog1.Location = new System.Drawing.Point(136, 16);
            this.profileCatalog1.Name = "profileCatalog1";
            this.profileCatalog1.SelectedProfile = "";
            this.profileCatalog1.Size = new System.Drawing.Size(62, 25);
            this.profileCatalog1.TabIndex = 4;
            this.profileCatalog1.SelectClicked += new System.EventHandler(this.profileCatalog1_SelectClicked);
            this.profileCatalog1.SelectionDone += new System.EventHandler(this.profileCatalog1_SelectionDone);
            // 
            // Form2
            // 
            this.structuresExtender.SetAttributeName(this, null);
            this.structuresExtender.SetAttributeTypeName(this, null);
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.structuresExtender.SetBindPropertyName(this, null);
            this.ClientSize = new System.Drawing.Size(208, 108);
            this.Controls.Add(this.profileCatalog1);
            this.Controls.Add(this.ColumnsProfileTextBox);
            this.Controls.Add(this.button1);
            this.Name = "Form2";
            this.Text = "斷面結構計算";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox ColumnsProfileTextBox;
        private Tekla.Structures.Dialog.UIControls.ProfileCatalog profileCatalog1;
    }
}