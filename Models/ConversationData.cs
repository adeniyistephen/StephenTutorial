using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlurasightBot.Models
{
    public class ConversationData
    {
        //Track if we have already ask user for name.

        public bool PromtedUserForName { get; set; } = false;
    }
}
