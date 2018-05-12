using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml.Linq;
namespace LuaDebugger.Plugins.S5CutsceneEditor
{
    class Cutscene
    {
        public List<Flight> Flights;


        public Cutscene()
        {
            this.Flights = new List<Flight>();
        }

        public void SaveCutscene(string filename, int startFromFlight, int startFromPoint)
        {
            XElement cp = new XElement("Flights");
            XElement la = new XElement("Flights");
            XElement lc = new XElement("Flights");
            XDocument xd = new XDocument(
                new XElement("root",
                     new XElement("CameraPos", cp),
                     new XElement("CameraLookAt", la),
                     new XElement("LuaFunctions", lc)));
            float timeOffset = 0;
            for (int i = startFromFlight; i < this.Flights.Count; i++ )
            {
                this.Flights[i].SmoothPaths();
                FlightXML flight;
                if (i == startFromFlight)
                {
                    timeOffset = this.Flights[i].CalculateTimes(timeOffset, startFromPoint);
                    flight = this.Flights[i].GetXML(startFromPoint);
                }
                else
                {
                    timeOffset = this.Flights[i].CalculateTimes(timeOffset, 0);
                    flight = this.Flights[i].GetXML();
                }
                cp.Add(flight.CameraPositionFlight);
                la.Add(flight.LookAtPositionFlight);
                lc.Add(flight.LuaCallbackFlight);
            }

            xd.Save(filename);
        }

        public void SaveCutscene(string filename, int startFromFlight)
        {
            this.SaveCutscene(filename, startFromFlight, 0);
        }

        public void SaveCutscene(string filename)
        {
            this.SaveCutscene(filename, 0, 0);
        }

        public XElement Serialize()
        {
            XElement cs = new XElement("Cutscene");
            foreach (Flight fl in Flights)
            {
                cs.Add(new XElement("Flight", fl.Serialize()));
            }
            return cs;
        }

        public static Cutscene Deserialize(XElement el)
        {
            el = el.Element("Cutscene");
            Cutscene cs = new Cutscene();
            foreach (XElement e in el.Elements("Flight"))
            {
                cs.Flights.Add(Flight.Deserialize(e));
            }
            return cs;
        }
    }
    
    class Flight
    {
        public List<FlightPoint> FlightPoints;
        public string Name;
        private int pointIdCounter = 0;
        public float StartTime;

        public Flight(string name)
        {
            this.Name = name;
            this.FlightPoints = new List<FlightPoint>();
        }

        public override string ToString()
        {
            return this.Name;
        }

        public int GetNextPointID()
        {
            pointIdCounter++;
            return pointIdCounter;
        }

        public XElement Serialize()
        {
            XElement f = new XElement("FlightPoints");
            XElement r = new XElement("Flight", new XElement("name", Name), f);
            for (int i=0; i<FlightPoints.Count; i++)
            {
                f.Add(new XElement("FlightPoint", FlightPoints[i].Serialize()));
            }
            return r;
        }

        public static Flight Deserialize(XElement el)
        {
            el = el.Element("Flight");
            Flight r = new Flight(el.Element("name").Value);
            foreach (XElement e in el.Element("FlightPoints").Elements("FlightPoint"))
            {
                r.FlightPoints.Add(FlightPoint.Deserialize(e, r.GetNextPointID()));
            }
            return r;
        }

        public void SmoothPaths()
        {
            List<Waypoint> validCP = new List<Waypoint>();
            List<Waypoint> validLP = new List<Waypoint>();
            foreach (FlightPoint fp in this.FlightPoints)
            {
                if (fp.CamPos.Active)
                    validCP.Add(fp.CamPos);
                if (fp.LookAtPos.Active)
                    validLP.Add(fp.LookAtPos);
            }

            foreach (List<Waypoint> validList in new List<List<Waypoint>> { validCP, validLP })
            {
                for (int i = 0; i < validList.Count - 1; i++)
                {
                    Waypoint[] wps = new Waypoint[] 
                        {
                            i == 0 ? validList[0] : validList[i-1],
                            validList[i],
                            validList[i+1],
                            i+2 == validList.Count ? validList[i+1] : validList[i+2]
                        };
                    CreateControlPoints(wps, 3);
                }
            }
        }

        private void CreateControlPoints(Waypoint[] wp, float smooth_value)
        {
            bool notFirst = !(wp[0].Equals(wp[1]));
            bool notLast = !(wp[2].Equals(wp[3]));

            float xc1 = (wp[0].Position.X + wp[1].Position.X) / 2.0f;
            float yc1 = (wp[0].Position.Y + wp[1].Position.Y) / 2.0f;
            float zc1 = (wp[0].Position.Z + wp[1].Position.Z) / 2.0f;
            float xc2 = (wp[1].Position.X + wp[2].Position.X) / 2.0f;
            float yc2 = (wp[1].Position.Y + wp[2].Position.Y) / 2.0f;
            float zc2 = (wp[1].Position.Z + wp[2].Position.Z) / 2.0f;
            float xc3 = (wp[2].Position.X + wp[3].Position.X) / 2.0f;
            float yc3 = (wp[2].Position.Y + wp[3].Position.Y) / 2.0f;
            float zc3 = (wp[2].Position.Z + wp[3].Position.Z) / 2.0f;
            //zc1-zc3

            float len1 = (float)Math.Sqrt((wp[1].Position.X - wp[0].Position.X) * (wp[1].Position.X - wp[0].Position.X) + (wp[1].Position.Y - wp[0].Position.Y) * (wp[1].Position.Y - wp[0].Position.Y) + (wp[1].Position.Z - wp[0].Position.Z) * (wp[1].Position.Z - wp[0].Position.Z));
            float len2 = (float)Math.Sqrt((wp[2].Position.X - wp[1].Position.X) * (wp[2].Position.X - wp[1].Position.X) + (wp[2].Position.Y - wp[1].Position.Y) * (wp[2].Position.Y - wp[1].Position.Y) + (wp[2].Position.Z - wp[1].Position.Z) * (wp[2].Position.Z - wp[1].Position.Z));
            float len3 = (float)Math.Sqrt((wp[3].Position.X - wp[2].Position.X) * (wp[3].Position.X - wp[2].Position.X) + (wp[3].Position.Y - wp[2].Position.Y) * (wp[3].Position.Y - wp[2].Position.Y) + (wp[3].Position.Z - wp[2].Position.Z) * (wp[3].Position.Z - wp[2].Position.Z));
            //plus z²

            float k1 = len1 / (len1 + len2);
            float k2 = len2 / (len2 + len3);

            float xm1 = xc1 + (xc2 - xc1) * k1;
            float ym1 = yc1 + (yc2 - yc1) * k1;
            float zm1 = zc1 + (zc2 - zc1) * k1;
            //zm1

            float xm2 = xc2 + (xc3 - xc2) * k2;
            float ym2 = yc2 + (yc3 - yc2) * k2;
            float zm2 = zc2 + (zc3 - zc2) * k2;
            //zm2

            // Resulting control points. Here smooth_value is mentioned
            // above coefficient K whose value should be in range [0...1].
            if (notFirst)
            {
                wp[1].OutTangent.X = xm1 + (xc2 - xm1) * smooth_value + wp[1].Position.X - xm1;
                wp[1].OutTangent.Y = ym1 + (yc2 - ym1) * smooth_value + wp[1].Position.Y - ym1;
                wp[1].OutTangent.Z = zm1 + (zc2 - zm1) * smooth_value + wp[1].Position.Z - zm1;
            }

            if (notLast)
            {
                wp[2].InTangent.X = xm2 + (xc2 - xm2) * smooth_value + wp[2].Position.X - xm2;
                wp[2].InTangent.Y = ym2 + (yc2 - ym2) * smooth_value + wp[2].Position.Y - ym2;
                wp[2].InTangent.Z = zm2 + (zc2 - zm2) * smooth_value + wp[2].Position.Z - zm2;
            }
        }
        public FlightXML GetXML(int startFromPoint)
        {
            string startTime = this.StartTime.ToString("e", CultureInfo.InvariantCulture);
            XElement cpPositios = new XElement("Positios");
            XElement laPositios = new XElement("Positios");
            XElement luaData = new XElement("Data");
            XElement cp = new XElement("Element",
                    new XElement("Time", startTime),
                    new XElement("Data", cpPositios));
            XElement la = new XElement("Element",
                    new XElement("Time", startTime),
                    new XElement("Data", laPositios));
            XElement lc = new XElement("Element",
                    new XElement("Time", startTime),
                    luaData);

            for (int i = startFromPoint; i < this.FlightPoints.Count; i++)
            {
                if (this.FlightPoints[i].CamPos.Active)
                    cpPositios.Add(this.FlightPoints[i].CamPos.GetXML());

                if (this.FlightPoints[i].LookAtPos.Active)
                    laPositios.Add(this.FlightPoints[i].LookAtPos.GetXML());

                if (this.FlightPoints[i].LuaCallback != "")
                    luaData.Add(this.FlightPoints[i].GetLuaXML());
            }

            return new FlightXML(cp, la, lc);
        }

        public FlightXML GetXML()
        {
            return this.GetXML(0);
        }

        public float CalculateTimes(float offset, int startFromPoint)
        {
            //fail: length is linear distance
            this.StartTime = offset;
            List<FlightPoint> fps = this.FlightPoints;
            fps[startFromPoint].CamPos.Time = 0;
            fps[startFromPoint].LookAtPos.Time = 0;
            float internOffset = 0;
            for (int i = startFromPoint + 1; i < this.FlightPoints.Count; i++)
            {
                float fullDistance = fps[i].getDistanceRelativeTo(fps[i - 1]);
                internOffset += fullDistance / fps[i].Speed;
                fps[i].CamPos.Time = internOffset;
                fps[i].LookAtPos.Time = internOffset;
            }
            return offset + internOffset;
        }
    }

    public struct FlightXML
    {
        public XElement CameraPositionFlight;
        public XElement LookAtPositionFlight;
        public XElement LuaCallbackFlight;
        public FlightXML(XElement cp, XElement la, XElement lc)
        {
            this.CameraPositionFlight = cp;
            this.LookAtPositionFlight = la;
            this.LuaCallbackFlight = lc;
        }
    }
    
    public class Waypoint
    {
        public Point3D Position, InTangent, OutTangent;
        public float Time;
        public bool Active;

        public Waypoint(Point3D position)
        {
            Position = position;
            Active = true;
            Time = 0;
            InTangent = OutTangent = Position;
        }

        public XElement GetXML()
        {
            return new XElement("Element",
                new XElement("Time", this.Time.ToString("e", CultureInfo.InvariantCulture)),
                new XElement("Data",
                    new XElement("Position", PointToXML(this.Position)),
                    new XElement("InTangent", PointToXML(this.Position.SubtractAndScale(this.InTangent, 1))),
                    new XElement("OutTangent", PointToXML(this.OutTangent.SubtractAndScale(this.Position, 1)))
                    ));

        }

        private XElement PointToXML(Point3D point)
        {
            return new XElement("Position",
                new XElement("x", point.X.ToString("e", CultureInfo.InvariantCulture)),
                new XElement("y", point.Y.ToString("e", CultureInfo.InvariantCulture)),
                new XElement("z", point.Z.ToString("e", CultureInfo.InvariantCulture)));
        }

        public XElement Serialize()
        {
            return new XElement("Waypoint", PointToXML(Position), new XElement("Active", Active.ToString()));
        }

        public static Waypoint Deserialize(XElement el)
        {
            el = el.Element("Waypoint");
            Waypoint r = new Waypoint(new Point3D(
                    float.Parse(el.Element("Position").Element("x").Value, CultureInfo.InvariantCulture),
                    float.Parse(el.Element("Position").Element("y").Value, CultureInfo.InvariantCulture),
                    float.Parse(el.Element("Position").Element("z").Value, CultureInfo.InvariantCulture)
            ));
            r.Active = el.Element("Active").Value == "True";
            return r;
        }
    }
    
    public class FlightPoint
    {
        public int ID;
        public Waypoint CamPos, LookAtPos;
        public string LuaCallback;
        public float Speed;
        public float CamPitch;
        public float CamYaw;
        public bool SpeedUseOnlyXY;

        public XElement GetLuaXML()
        {
            return new XElement("Element",
                new XElement("Time", this.CamPos.Time.ToString("e", CultureInfo.InvariantCulture)),
                new XElement("FuncName", this.LuaCallback)
            );
        }
        public FlightPoint(S5CameraInfo camera, int id)
        {
            ID = id;
            LookAtPos = new Waypoint(camera.Point3D.MoveBy((float)(camera.PitchAngle * Math.PI / 180), (float)(camera.YawAngle * Math.PI / 180), 1000));
            CamPos = new Waypoint(camera.Point3D);
            LuaCallback = "";
            Speed = 800;
            SpeedUseOnlyXY = false;

            CamPitch = camera.PitchAngle;
            CamYaw = camera.YawAngle;
        }
        private FlightPoint()
        {

        }

        public XElement Serialize()
        {
            return new XElement("FlightPoint", new XElement("CamPos", CamPos.Serialize()), new XElement("LookAt", LookAtPos.Serialize()),
                new XElement("callback", LuaCallback), new XElement("pitch", CamPitch.ToString(CultureInfo.InvariantCulture)),
                new XElement("yaw", CamYaw.ToString(CultureInfo.InvariantCulture)), new XElement("speed", Speed.ToString(CultureInfo.InvariantCulture)),
                new XElement("speedOnlyXY", SpeedUseOnlyXY.ToString())
                );
        }

        public static FlightPoint Deserialize(XElement el, int id)
        {
            el = el.Element("FlightPoint");
            FlightPoint r = new FlightPoint();
            r.CamPos = Waypoint.Deserialize(el.Element("CamPos"));
            r.LookAtPos = Waypoint.Deserialize(el.Element("LookAt"));
            r.LuaCallback = el.Element("callback").Value;
            r.CamPitch = float.Parse(el.Element("pitch").Value, CultureInfo.InvariantCulture);
            r.CamYaw = float.Parse(el.Element("yaw").Value, CultureInfo.InvariantCulture);
            r.Speed = float.Parse(el.Element("speed").Value, CultureInfo.InvariantCulture);
            if (el.Element("speedOnlyXY") != null)
            {
                r.SpeedUseOnlyXY = el.Element("speedOnlyXY").Value == "True";
            }
            r.ID = id;
            return r;
        }

        public float getDistanceRelativeTo(FlightPoint p)
        {
            float dx = p.CamPos.Position.X - this.CamPos.Position.X;
            float dy = p.CamPos.Position.Y - this.CamPos.Position.Y;
            float dz = p.CamPos.Position.Z - this.CamPos.Position.Z;
            float fullDistance = 0;
            if (this.SpeedUseOnlyXY)
            {
                fullDistance = (float)Math.Sqrt(dx * dx + dy * dy);
            }
            else
            {
                fullDistance = (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
            }
            return fullDistance;
        }
    }
}
