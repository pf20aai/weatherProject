using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherPrediction
{

    /// <summary>
    /// This File holds all of the Enums used on this project
    /// New Enums can be added by copying a previous one and just ammending the values
    /// To call an Enum because this is held at the top level simply call it with the Name of the enum
    /// E.G Counties.xxxx
    /// </summary>
    /// 


    public enum Counties
    {
        Avon,
        Bedfordshire,
        Berkshire,
        Buckinghamshire,
        Cambridgeshire,
        Cheshire,
        Cornwall,
        Cumbria,
        Derbyshire,
        Devon,
        Dorset,
        Durham,
        Essex,
        Gloucestershire,
        GreaterLondon,
        Hampshire,
        Herefordshire,
        Hertfordshire,
        IsleOfWight,
        Kent,
        Lancashire,
        Leicestershire,
        Lincolnshire,
        Merseyside,
        Norfolk,
        Northamptonshire,
        Northumberland,
        Nottinghamshire,
        Oxfordshire,
        Rutland,
        Schools,
        Shropshire,
        Somerset,
        Staffordshire,
        Suffolk,
        Surrey,
        Sussex,
        TyneAndWear,
        Warwickshire,
        WestMidlands,
        Wiltshire,
        Worcestershire,
        Yorkshire
    }

    public enum WeatherConditions
    {
        Sunny,
        PartiallyCloudy,
        Cloudy,
        Overcast,
        Drizzle,
        Rain,
        Stormy,
        Snow
    }

    public enum permissionTypes
    {
        User,
        Admin
    }
}
