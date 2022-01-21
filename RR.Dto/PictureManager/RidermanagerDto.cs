using System;
using System.Collections.Generic;
using System.Text;

namespace RR.Dto
{
    public class RidermanagerDto
    {
        public RidermanagerDto()
        {
            this.SocialLinks = new List<RiderSocialLinksDto>();
        }
        public int RiderId { get; set; }
        public List<RiderSocialLinksDto> SocialLinks { get; set; }
        public void addSocial(int count)
        {
            for (int i = 0; i < count; i++)
            {
                SocialLinks.Add(new RiderSocialLinksDto());
            }
        }
    }
    public class RiderSocialLinksDto
    {
        public long Id { get; set; }
        public string Sociallink { get; set; }
        public string Icon { get; set; }
        public SocialType Type { get; set; }

    }
    public enum SocialType
    {
        Facebook,
        Google,
        Youtube,
        Instagram,
        linkedin,
        Twitter
    }
}
