namespace LastKey_Domain.Entities.DTOs;

public class FreeFaceResponse
{
    public string request_id { get; set; }

    public float confidence { get; set; }

    public object thresholds { get; set; }

    public string image_id1 { get; set; }

    public string image_id2 { get; set; }

    public object[] faces1 { get; set; }
    
    public object[] faces2 { get; set; }

    public int time_used { get; set; }

    public string error_message { get; set; }
}