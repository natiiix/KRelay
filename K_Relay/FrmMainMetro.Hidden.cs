using System;

namespace K_Relay
{
    partial class FrmMainMetro
    {
        private void tglStartByDefault_CheckedChanged(object sender, EventArgs e)
        {
            lblStartByDefault.Text = tglStartByDefault.Checked.ToString();
        }

        private void lblStartByDefault_Click(object sender, EventArgs e)
        {
            tglStartByDefault.Checked = !tglStartByDefault.Checked;
        }
    }
}
