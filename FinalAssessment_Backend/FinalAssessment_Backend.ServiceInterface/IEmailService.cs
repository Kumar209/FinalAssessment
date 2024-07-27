using FinalAssessment_Backend.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalAssessment_Backend.ServiceInterface
{
    public interface IEmailService
    {
        public Task SendEmailAsync(MailRequest mailrequest);
    }
}
