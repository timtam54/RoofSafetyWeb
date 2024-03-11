
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace RoofSafety.Models
{
    
    public class InspectionRpt
    {
        public List<VersionRpt> Versions { get; set; } 
        public string Title { get; set; }   
        public DateTime InspDate { get; set; }
        public string Areas { get; set; }
        public string Callibration { get; set; }
        public string Tests { get; set; }
        public string Inspector { get; set; }

        public string Instrument { get; set; }
        public List<InspEquipTest> Items { get; set; }
        public List<string> InspItems { get; set; }
        public string Photo { get; set; }

        public int id { get; set; }

    }
    public class InspEquipTest
    {
        public int? ETID { get; set; }
        public int Qty { get; set; }

        public List<string> SerialNos
        {
            get
            {
                int sn = Convert.ToInt32((SNSuffix==null)?1:SNSuffix);
                List<string> ret = new List<string>();
                for (int i = 0; i < Qty; i++)
                {
                    ret.Add(SerialNo+ (sn + i).ToString());

                }
                return ret;
            }
        }
        public bool Pass { get; set; }
        public string Result
        {
            get
            {
                int FailCount = TestResult.Where(i => i.PassFail == 0).Count();
                if (FailCount > 0)
                {
                    if (TestResult.Max(i=>i.Severity)==0)
                        return "Recommendation (max severity is 0)";

                    return "Non-Compliant";
                }
                int RecCount = TestResult.Where(i => i.PassFail == 2).Count();
                if (RecCount > 0)
                    return "Recommendation";
                return "Compliant";
            }
        }
        public string Risk
        {
            get
            {
                int FailCount = TestResult.Where(i => i.PassFail == 0).Count();
                if (FailCount > 0)
                {
                    int? Sev = TestResult.Where(i => i.PassFail == 0).Max(i => i.Severity);
                    if (Sev != null)
                    {
                        if (Sev == 0)
                            return "Recommendation";
                        return "Risk " + Sev.ToString();
                    }
                    return "Risk";

                }
                int RecCount = TestResult.Where(i => i.PassFail == 2).Count();
                if (RecCount > 0)
                    return "Recommendation";
                return "Maintained";
                //return (TestResult.Where(i => i.PassFail == false).Count() == 0) ? "Maintained" : "Risk";
            }
        }

        public string? Comment;

        public List<string>? Hazards;
        /*        {
                    get

                        ;


                    set ; 
                }*/
        public class Exp

        {
            public List<Ep> Eps { get; set; }
        }
            public class Ep
            {

                public string? ASTest { get; set; }
            public string colour {  get; set; }
                public string? P2 { get; set; }
            }
        

      public  static string getcolour(int Severity)
        {
            if (Severity <= 3)
                return "#0BDA51";// LimeGreen";
            if (Severity <= 6)
                return "#0096FF";// "royalblue";
            if (Severity <= 12)
                return "Yellow";
            return "#FF5733";// "Red";
        }


        public int MaxRisk
        {
            get
            {
                
                if (TestResult == null)
                    return 0;
                if (TestResult.Count(i => i.PassFail == 0) == 0)
                    return 0;
                var sev = TestResult.Where(i => i.PassFail == 0).OrderByDescending(i => i.Severity).FirstOrDefault();
                return sev!.Severity!.Value;
            }
        }
        public Exp Explanation
        {
            get
            {
                Exp ret = new Exp();
                ret.Eps = new List<Ep>();
                if (TestResult.Count(i => i.PassFail == 0) == 0)
                {
                    if (TestResult.Count(i => i.PassFail == 2) == 0)
                    {
                        //passed
                        foreach (var tr in TestResult.ToList())
                        {
                            Ep ep = new Ep();
                            ep.P2 = "";// ret.P1 = "";
                            ep.ASTest = "";
                            ep.colour = "gray";
                            if (tr.FailReason != null)
                            {
                                foreach (var fr in tr.FailReason)
                                {
                                    if (ep.P2 == "")
                                        ep.P2 = fr + "(Pass)";
                                    else
                                        ep.P2 = ep.P2 + ", " + fr + "(Pass)";
                                }

                            }
                            if (ep.ASTest == "")
                                ep.ASTest = tr.Test;
                            else
                                ep.ASTest = ep.ASTest + ", " + tr.Test;

                            //ret.P2 = ret.P2 /*+ " " + tr.Test*/ + " (Pass)";
                            if (tr.Comment != null && tr.Comment != "")
                            {
                                if (ep.P2 == "" || ep.P2 == null)
                                    ep.P2 = tr.Comment;
                                else
                                    ep.P2 = ep.P2 + " - " + tr.Comment;
                            }
                            ret.Eps.Add(ep);
                        }
                    }
                    else
                    {
                        //recommendations
                        foreach (var tr in TestResult.Where(i=>i.PassFail==2).ToList())
                        {
                            Ep ep = new Ep();
                            ep.P2 = "";// ret.P1 = "";
                            ep.ASTest = "";
                            ep.colour = "orange";
                            if (tr.FailReason != null)
                            {
                                foreach (var fr in tr.FailReason)
                                {
                                    if (ep.P2 == "")
                                        ep.P2 = fr + "(Recommend)";
                                    else
                                        ep.P2 = ep.P2 + ", " + fr + "(Recommend)";
                                }
                            }
                            if (ep.ASTest == "")
                                ep.ASTest = tr.Test;
                            else
                                ep.ASTest = ep.ASTest + ", " + tr.Test;

                            //ret.P2 = ret.P2 /*+ " " + tr.Test*/ + " (Pass)";
                            if (tr.Comment != null && tr.Comment != "")
                            {
                                if (ep.P2 == "" || ep.P2 == null)
                                    ep.P2 = tr.Comment;
                                else
                                    ep.P2 = ep.P2 + " - " + tr.Comment;
                            }
                            ret.Eps.Add(ep);
                        }
                    }
                   
                }
                else
                {
                    foreach (var tr in TestResult.Where(i => i.PassFail == 0).ToList())
                    {
                        Ep ep = new Ep();
                        ep.P2 = "";// ret.P1 = "";
                        ep.ASTest = "";
                        ep.colour = getcolour(tr.Severity.Value);
                        if (ep.ASTest == "")
                        {
                            ep.ASTest = tr.Test;// + "(Severity:" + tr.Severity.ToString() + ")";
                            
                        }
                        else
                        {
                            ep.ASTest = ep.ASTest + " " + tr.Test;// + "(Severity:" + tr.Severity.ToString() +  ")";
                        }
                        if (ep.P2 == "")
                            ;// ep.P2 = "Fail";
                        else
                            ep.P2 = ep.P2 + ", ";// Fail";
                        int ii = 0;
                        //ret.P2 = tr.Test;
                        if (tr.FailReason.Count== 0)
                        
                            ep.P2 = "No Fail Reason specified" ;
                          else
                            { 
                            if (ep.P2 != "")
                               ep.P2=ep.P2 + " - ";
                            foreach (var fr in tr.FailReason)
                            {
                                if (ii == 0)
                                    ep.P2 = ep.P2 + fr;
                                else
                                    ep.P2 = ep.P2 + "," + fr;
                                ii++;
                            }
                        }
                        if (tr.Comment!=null)
                            ep.P2 = ep.P2 + ". " + tr.Comment;
                        ret.Eps.Add(ep);
                    }
                }
                return ret ;
                //return (TestResult.Where(i => i.PassFail == false).Count() == 0) ? "Maintained" : "Risk";
            }
        }
     
        public int id { get; set; }

        [Display(Name = "Inspection")]
        public int InspectionID { get; set; }
        public string? Location { get; set; }
        public string? Notes { get; set; }
        public string? EquipName { get; set; }
        public EquipType? EquipType { get; set; }


        public string? RequiredControls { get; set; }

        public List<TestResult>? TestResult { get; set; }


        public string? Manufacturer { get; set; }
        public string? Installer { get; set; }
        public string? Rating { get; set; }
        public string? SerialNo { get; set; }

        public string? ItemNo { get; set; }
        public int? SNSuffix { get; set; }
        public DateTime? WithdrawalDate { get; set; }

        public List<InspPhoto>? Photos { get; set; }
    }

    public class TestResult
    {
        public int? iettid { get; set; }
        public string? Test { get; set; }
        public int PassFail { get; set; }

        public int? EquipTypeTestID { get; set; }

        public int? Severity { get; set; } 

        public List<string>? FailReason { get; set; }

        public string? Comment { get; set; }

    }



}
