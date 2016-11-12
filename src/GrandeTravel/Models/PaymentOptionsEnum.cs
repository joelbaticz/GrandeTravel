using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel;

namespace GrandeTravel.Models
{
    public enum PaymentOptionsEnum
    {
        [Description("PayPal")]
        PayPal,
        [Description("Credit Card")]
        CreditCard,
        [Description("Debit Card")]
        DebitCard
    }
}
