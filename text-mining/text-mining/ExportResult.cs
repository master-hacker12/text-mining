using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using EP;
using EP.Semantix;

namespace text_mining
{
    static class ExportResult
    {
        public static void ProcessXml(string txt, string xml, ref Processor proc)
        {
            // обрабатываем текст
           
            var ar = proc.Process(new SourceOfAnalysis(txt));

            XmlWriterSettings ws = new XmlWriterSettings() { Encoding = Encoding.UTF8, Indent = true };
            using (XmlWriter xml_out = XmlWriter.Create(xml, ws))
            {
                xml_out.WriteStartDocument();
                if (string.IsNullOrEmpty(txt) || ar == null)
                    xml_out.WriteElementString("error", "empty text");
                else
                {
                    xml_out.WriteStartElement("result");
                    xml_out.WriteAttributeString("version", ProcessorService.Version.ToString());
                    xml_out.WriteAttributeString("lang", ar.BaseLanguage.ToString());
                    xml_out.WriteAttributeString("chars", txt.Length.ToString());

                    // назначаем ID для сущностей, чтобы корректно оформлять ссылки
                    int id = 0; foreach (var e in ar.Entities) e.Tag = ++id;

                    foreach (Referent e in ar.Entities)
                    {
                        xml_out.WriteStartElement("entity");
                        xml_out.WriteAttributeString("id", e.Tag.ToString());
                        xml_out.WriteAttributeString("type", e.TypeName);
                        foreach (Slot s in e.Slots)
                        {
                            xml_out.WriteStartElement("attr");
                            xml_out.WriteAttributeString("name", s.TypeName);
                            if (s.Value is Referent)
                            {
                                xml_out.WriteAttributeString("ref", (s.Value as Referent).Tag.ToString());
                                xml_out.WriteString(_corrXmlString((s.Value as Referent).ToString(true)));
                            }
                            else if (s.Value is string)
                                xml_out.WriteString(_corrXmlString(s.Value as string));
                            xml_out.WriteEndElement();
                        }


                       // xml_out.WriteStartElement("Part_text");
                       //string x = "";
                       // foreach (var v in ar.Entities)
                       //     foreach (var y in v.Occurrence)
                       //     {
                       //         x += y.ToString();
                       //     }
                       // xml_out.WriteString(_corrXmlString(x.ToString()));
                       // xml_out.WriteEndElement();


                        xml_out.WriteEndElement();
                       

                    }

                    int i = 1;
                    xml_out.WriteStartElement("Tokens");
                    for (Token t = ar.FirstToken; t != null; t = t.Next)
                    {


                        xml_out.WriteStartElement("Token");
                        xml_out.WriteAttributeString("id", i.ToString());
                        xml_out.WriteAttributeString("BeginChar", t.BeginChar.ToString());

                        xml_out.WriteAttributeString("EndChar", t.EndChar.ToString());
                        xml_out.WriteAttributeString("LengthChar", t.LengthChar.ToString());

                        try
                        {
                            if ((t.EndChar - t.BeginChar) != 0)
                                xml_out.WriteValue(t.ToString());

                        }
                        catch (Exception ee)
                        {
                            // xml_out.WriteValue("null");
                        }
                        xml_out.WriteEndElement();
                        i++;


                    }
                    xml_out.WriteEndElement();

                    xml_out.WriteStartElement("processors");
                    i = 1;

                    foreach (var p in proc.Analyzers)
                    {
                        xml_out.WriteStartElement("processor");
                        xml_out.WriteAttributeString("id", i.ToString());
                        xml_out.WriteValue(p.ToString());
                        xml_out.WriteEndElement();
                        i++;
                    }


                    xml_out.WriteEndElement();
                    xml_out.WriteEndDocument();
                }
            }
        }

        // к сожалению, приходится корректировать строку, а то разное бывает, что портит XML
        static string _corrXmlString(string txt)
        {
            if (txt == null) return "";
            foreach (var c in txt)
                if ((int)c < 0x20 && c != '\r' && c != '\n' && c != '\t')
                {
                    StringBuilder tmp = new StringBuilder(txt);
                    for (int i = 0; i < tmp.Length; i++)
                    {
                        char ch = tmp[i];
                        if ((int)ch < 0x20 && ch != '\r' && ch != '\n' && ch != '\t')
                            tmp[i] = ' ';
                    }
                    return tmp.ToString();
                }
            return txt;
        }

    }
}
