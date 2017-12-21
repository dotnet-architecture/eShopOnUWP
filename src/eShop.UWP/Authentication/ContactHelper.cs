using System;
using System.Threading.Tasks;

using Windows.System;
using Windows.ApplicationModel.Contacts;

namespace eShop.UWP.Authentication
{
    static public class ContactHelper
    {
        static public async Task<Contact> CreateContactFromLogonUserAsync()
        {
            string userName = AppSettings.Current.UserName;
            userName = userName ?? "Unknown";

            var contact = await CreateContactFromCurrentUserAsync();
            if (contact.DisplayName.Equals(userName, StringComparison.OrdinalIgnoreCase))
            {
                return contact;
            }

            return new Contact
            {
                Name = userName,
                DisplayNameOverride = userName,
                SourceDisplayPicture = null
            };
        }

        static public async Task<Contact> CreateContactFromCurrentUserAsync()
        {
            var user = Activation.ActivationService.CurrentUser;
            if (user != null)
            {
                return await CreateContactAsync(user);
            }
            return CreateDefaultContact();
        }

        static public async Task<Contact> CreateContactAsync(User user)
        {
            var accountName = await user.GetPropertyAsync(KnownUserProperties.AccountName) as String;
            var firstName = await user.GetPropertyAsync(KnownUserProperties.FirstName) as String;
            var lastName = await user.GetPropertyAsync(KnownUserProperties.LastName) as String;
            var displayName = await user.GetPropertyAsync(KnownUserProperties.DisplayName) as String;
            var pictureStream = await user.GetPictureAsync(UserPictureSize.Size64x64);

            if (String.IsNullOrEmpty(displayName))
            {
                displayName = $"{firstName} {lastName}".Trim();
                if (String.IsNullOrEmpty(displayName))
                {
                    displayName = accountName;
                    if (String.IsNullOrEmpty(displayName))
                    {
                        return CreateDefaultContact();
                    }
                }
            }

            return new Contact
            {
                Name = accountName,
                FirstName = firstName,
                LastName = lastName,
                DisplayNameOverride = displayName,
                SourceDisplayPicture = pictureStream
            };
        }

        static public Contact CreateDefaultContact()
        {
            return new Contact
            {
                Name = "Lacey Heath",
                FirstName = "Lacey",
                LastName = "Heath",
                DisplayNameOverride = "Lacey Heath",
                SourceDisplayPicture = null
            };
        }
    }
}
