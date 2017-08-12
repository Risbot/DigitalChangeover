using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace HA.MVVMClient.Infrastructure
{
    public class PrintPrewiev : DocumentViewer
    {
        protected override void OnPrintCommand()
        {
            PrintDialog dialog = new PrintDialog();
            if (dialog.ShowDialog() == true)
                dialog.PrintDocument(Document.DocumentPaginator, Description);
        }

        public string Description { get; set; }
    }
}
