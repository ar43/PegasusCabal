namespace DataTools
{
	partial class Form1
	{
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
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
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			button1 = new Button();
			textBox1 = new TextBox();
			comboBox1 = new ComboBox();
			textBox2 = new TextBox();
			button2 = new Button();
			button3 = new Button();
			button4 = new Button();
			SuspendLayout();
			// 
			// button1
			// 
			button1.Location = new Point(203, 215);
			button1.Name = "button1";
			button1.Size = new Size(75, 23);
			button1.TabIndex = 0;
			button1.Text = "Copy JSON";
			button1.UseVisualStyleBackColor = true;
			button1.Click += button1_Click;
			// 
			// textBox1
			// 
			textBox1.Location = new Point(34, 54);
			textBox1.Name = "textBox1";
			textBox1.Size = new Size(244, 23);
			textBox1.TabIndex = 1;
			// 
			// comboBox1
			// 
			comboBox1.FormattingEnabled = true;
			comboBox1.Location = new Point(24, 215);
			comboBox1.Name = "comboBox1";
			comboBox1.Size = new Size(121, 23);
			comboBox1.TabIndex = 2;
			// 
			// textBox2
			// 
			textBox2.Location = new Point(41, 311);
			textBox2.Multiline = true;
			textBox2.Name = "textBox2";
			textBox2.Size = new Size(223, 184);
			textBox2.TabIndex = 3;
			textBox2.TextChanged += textBox2_TextChanged;
			// 
			// button2
			// 
			button2.Location = new Point(326, 538);
			button2.Name = "button2";
			button2.Size = new Size(125, 23);
			button2.TabIndex = 4;
			button2.Text = "Fix CharInit JSON";
			button2.UseVisualStyleBackColor = true;
			button2.Click += button2_Click;
			// 
			// button3
			// 
			button3.Location = new Point(361, 366);
			button3.Name = "button3";
			button3.Size = new Size(75, 23);
			button3.TabIndex = 5;
			button3.Text = "button3";
			button3.UseVisualStyleBackColor = true;
			button3.Click += button3_Click;
			// 
			// button4
			// 
			button4.Location = new Point(361, 310);
			button4.Name = "button4";
			button4.Size = new Size(75, 23);
			button4.TabIndex = 6;
			button4.Text = "button4";
			button4.UseVisualStyleBackColor = true;
			button4.Click += button4_Click_1;
			// 
			// Form1
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(463, 573);
			Controls.Add(button4);
			Controls.Add(button3);
			Controls.Add(button2);
			Controls.Add(textBox2);
			Controls.Add(comboBox1);
			Controls.Add(textBox1);
			Controls.Add(button1);
			Name = "Form1";
			Text = "Form1";
			Load += Form1_Load;
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private Button button1;
		private TextBox textBox1;
		private ComboBox comboBox1;
		private TextBox textBox2;
		private Button button2;
		private Button button3;
		private Button button4;
	}
}
