using System;
using brid_emitter.Models;

namespace brid_emitter.Contracts
{
    public class Citizen
    {
        public string Uuid;
        public string Rg;
        public string CPF;
        public string Name;
        public string ShortName;
        public string Address;
        public string Sex;
        public string Race;
        public DateTime DateOfBirth;
        public string Naturality;
        public CivilStatus CivilStatus;
        public string MothersName;
        public string FathersName;
        public string Scholarity;
        public string Job;
        public Contact Contact;
    }

    public enum CivilStatus
    {
        Single,
        Married,
        Widow,
    }
}