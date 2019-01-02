using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace oSEAT2RefereeAssistant
{
    public class SelectAllTextBox : TextBox
    {
        protected override void OnClick(EventArgs e)
        {
            SelectAll();
            base.OnClick(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                SelectAll();
                e.Handled = true;
            }
            base.OnKeyDown(e);
        }
    }
}
