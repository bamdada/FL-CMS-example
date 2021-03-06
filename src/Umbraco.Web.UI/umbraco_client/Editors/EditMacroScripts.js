Umbraco.Sys.registerNamespace("Umbraco.Editors");

(function ($) {

    Umbraco.Editors.EditMacroScripts = base2.Base.extend({
        //private methods/variables
        _opts: null,

        // Constructor
        constructor: function(opts) {
            // Merge options with default
            this._opts = $.extend({
                

                // Default options go here
            }, opts);
        },

        init: function () {
            //setup UI elements
            var self = this;

            //bind to the save event
            this._opts.saveButton.click(function () {
                self.doSubmit();
            });
        },
        
        doSubmit: function () {
            var self = this;

            jQuery('#errorDiv').hide();

            var fileName = this._opts.nameTxtBox.val();
            var codeVal = this._opts.editorSourceElement.val();
            //if CodeMirror is not defined, then the code editor is disabled.
            if (typeof (CodeMirror) != "undefined") {
                codeVal = UmbEditor.GetCode();
            }
            umbraco.presentation.webservices.codeEditorSave.SaveDLRScript(
                fileName, self._opts.originalFileName, codeVal, self._opts.skipTestingCheckBox.is(':checked'),
                function (t) { self.submitSucces(t); },
                function (t) { self.submitFailure(t); });

        },

        submitSucces: function(t) {
            if (t != 'true') {
                top.UmbSpeechBubble.ShowMessage('error', 'Saving scripting file failed', '');
                jQuery('#errorDiv').html('<p><a href="#" style="position: absolute; right: 10px; top: 10px;" onclick=\'closeErrorDiv()\'>Hide Errors</a><strong>Error occured</strong></p><p>' + t + '</p>');
                jQuery('#errorDiv').slideDown('fast');
            }
            else {
                top.UmbSpeechBubble.ShowMessage('save', 'Scripting file saved', '');
            }


            var newFilePath = this._opts.nameTxtBox.val();
            UmbClientMgr.mainTree().setActiveTreeType('python');
            //we need to pass in the newId parameter so it knows which node to resync after retreival from the server
            UmbClientMgr.mainTree().syncTree("-1,init," + this._opts.originalFileName, true, null, newFilePath);
            //set the original file path to the new one
            this._opts.originalFileName = newFilePath;
        },

        submitFailure: function(t) {
            top.UmbSpeechBubble.ShowMessage('warning', 'Scripting file could not be saved', '');
        }
    });
})(jQuery);