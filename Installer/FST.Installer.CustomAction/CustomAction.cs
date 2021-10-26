using Microsoft.Deployment.WindowsInstaller;
using System;
using System.DirectoryServices.AccountManagement;
using System.Windows.Forms;

namespace FST.Installer.CustomAction
{
    public class CustomActions
    {
        [CustomAction]
        public static ActionResult WindowsCredentialValidation(Session session)
        {
            try
            {

                var valid = false;
                using (var context = new PrincipalContext(ContextType.Domain))
                {
                    valid = context.ValidateCredentials(
                        session[Const.WEB_APP_POOL_IDENTITY_NAME],
                        session[Const.WEB_APP_POOL_IDENTITY_PWD]
                    );
                }

                if (!valid)
                {
                    MessageBox.Show("Windows credential is not valid", "Error");
                }

                session[Const.WEB_APP_POOL_IDENTITY_IS_VALID] = valid ? "True" : "False";
            }
            catch (Exception e)
            {
                MessageBox.Show($"Can't check credential: {e.Message}", "Error");
                return ActionResult.Success;
            }

            return ActionResult.Success;
        }
    }
}
