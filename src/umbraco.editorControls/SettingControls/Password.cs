using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using umbraco.cms.businesslogic.datatype;

namespace umbraco.editorControls.SettingControls
{
    public class Password : DataEditorSettingType
    {
        private TextBox tb = new TextBox();

        public override string Value
        {
            get
            {
                return tb.Text;
            }
            set
            {
                tb.Text = value;
                tb.Attributes["value"] = value;
            }
        }


        public override System.Web.UI.Control RenderControl(DataEditorSetting sender)
        {
            tb.ID = sender.GetName();

            tb.TextMode = TextBoxMode.Password;
            tb.CssClass = "guiInputText guiInputStandardSize";

            if (string.IsNullOrEmpty(tb.Text) && !string.IsNullOrEmpty(DefaultValue))
                tb.Text = DefaultValue;

            return tb;
        }
    }
}