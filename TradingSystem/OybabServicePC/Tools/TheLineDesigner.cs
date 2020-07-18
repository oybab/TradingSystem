using System;
using System.Windows.Forms.Design;

namespace Oybab.ServicePC.Tools.Design
{
    internal sealed class BevelLineDesigner : System.Windows.Forms.Design.ControlDesigner
	{
		public BevelLineDesigner()
		{
			
		}
		public override SelectionRules SelectionRules
		{
			get
			{
				SelectionRules rules;

				rules = base.SelectionRules;
				
				return rules;
			}
		}
	}
}
