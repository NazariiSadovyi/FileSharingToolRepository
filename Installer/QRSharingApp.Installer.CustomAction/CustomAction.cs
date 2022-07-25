using Microsoft.Deployment.WindowsInstaller;
using NetFwTypeLib;
using QRSharingApp.Shared;
using System;
using System.DirectoryServices.AccountManagement;
using System.Windows.Forms;

namespace QRSharingApp.Installer.CustomAction
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

        [CustomAction]
        public static ActionResult UpdateFirewallRule(Session session)
        {
            try
            {
                var port = SharedConstants.Port;
                var newRuleName = $"FileSharingTool for port {port}";

                Type tNetFwPolicy2 = Type.GetTypeFromProgID("HNetCfg.FwPolicy2");
                var fwPolicy2 = (INetFwPolicy2)Activator.CreateInstance(tNetFwPolicy2);

                try
                {
                    fwPolicy2.Rules.Item(newRuleName);
                    return ActionResult.Success;
                }
                catch
                {
                    // if it failed than there is no such rule yet
                }

                var currentProfiles = fwPolicy2.CurrentProfileTypes;

                // Let's create a new rule
                var inboundRule = (INetFwRule2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
                inboundRule.Enabled = true;
                //Allow through firewall
                inboundRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
                //Profiles
                inboundRule.Profiles = (int)NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_PRIVATE
                    | (int)NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_DOMAIN | (int)NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_PUBLIC;
                //Using protocol TCP
                inboundRule.Protocol = 6; // TCP
                inboundRule.LocalPorts = port.ToString();
                //Name of rule
                inboundRule.Name = newRuleName;
                // ...//
                inboundRule.Profiles = currentProfiles;

                // Now add the rule
                INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
                firewallPolicy.Rules.Add(inboundRule);
            }
            catch (Exception e)
            {
                MessageBox.Show($"Can't update firewall rule: {e.Message}", "Error");
                return ActionResult.Failure;
            }

            return ActionResult.Success;
        }
    }
}
