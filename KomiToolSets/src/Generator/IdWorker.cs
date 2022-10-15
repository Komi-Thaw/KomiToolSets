namespace KomiToolSets.StringPack;

public class IdWorker
{
    private static long Machine_Id; // Machine ID

    private static long Data_Center_Id = 0L; //Data ID

    private static long Sequence = 0L; //Counting starts from zero

    private static long rdepoch = 687888001020L; // Unique Time Epoch

    private static long Machine_Id_Bits = 5L; //Number of machine code bytes

    private static long Data_Center_Bits = 5L; //Number of data bytes

    public static long Max_Machine_Id = -1L ^ -1L << (int)Machine_Id_Bits; //Maximum machine ID

    private static long Max_Data_Center_Id = -1L ^ (-1L << (int)Data_Center_Bits); //Maximum data ID

    private static long Sequence_Bits = 12L; //Counter bytes, 12 bytes are used to store the counter code         

    private static long
        Machine_Id_Shift =
            Sequence_Bits; //The left shift number of machine code data is the number of bits occupied by the counter behind 

    private static long Data_Center_Id_Shift = Sequence_Bits + Machine_Id_Bits;

    private static long
        Timestap_Left_Shift =
            Sequence_Bits + Machine_Id_Bits +
            Data_Center_Bits; //The number of bits shifted to the left of the timestamp is the machine code + the total number of bytes of the counter + the number of data bytes 

    private static long Sequence_Mask = -1L ^ -1L << (int)
        Sequence_Bits; //The count can be generated within one microsecond, if it reaches this value, it will wait until the next microsecond is in progress. 

    private static long Last_Time_Stamp = -1L; //Last timestamp

    private static object Sync = new();

    private static IdWorker? flake;

    /// <summary>
    /// 生成实例(支持多线程)
    /// </summary>
    /// <returns></returns>
    public static IdWorker Instance()
    {
        if (flake is not null) return flake;
        lock (Sync)
        {
            return flake ?? new IdWorker();
        }
    }

    public IdWorker()
    {
        Snowflakes(0L, -1L);
    }

    public IdWorker(long machineId)
    {
        Snowflakes(machineId, -1L);
    }


    public IdWorker(long machineId, long datacenterId)
    {
        Snowflakes(machineId, datacenterId);
    }

    private void Snowflakes(long machineId, long datacenterId)
    {
        if (machineId >= 0)
        {
            if (machineId > Max_Machine_Id)
            {
                throw new Exception(" Machine code ID is illegal ");
            }

            Machine_Id = machineId;
        }

        if (datacenterId < 0) return;

        if (datacenterId > Max_Data_Center_Id)
        {
            throw new Exception(" Data center ID is illegal ");
        }

        Data_Center_Id = datacenterId;
    }

    ///<summary> 
    ///生成时间戳
    ///</summary> 
    ///<returns>time to milliseconds</returns> 
    private static long GetTimestamp()
    {
        return (long)(DateTimeOffset.UtcNow - new DateTime(1985, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
    }

    ///<summary> 
    ///获取下一个时间戳
    ///</summary> 
    ///<param name="lastTimestamp"></param> 
    ///<returns></returns> 
    private static long GetNextTimestamp(long lastTimestamp)
    {
        var timestamp = GetTimestamp();
        var count = 0;
        while
            (timestamp <=
             lastTimestamp) //Get the new time here, there may be errors, this algorithm is the same as comb for strict requirements on machine time
        {
            count++;
            if (count > 10)
                throw new Exception("机器时间未校正!");
            Thread.Sleep(1);
            timestamp = GetTimestamp();
        }

        return timestamp;
    }

    ///<summary> 
    /// 获取ID
    ///</summary> 
    ///<returns></returns> 
    public long Generate()
    {
        lock (Sync)
        {
            var timestamp = GetTimestamp();
            if (Last_Time_Stamp == timestamp)
            {
                //Generate ID 
                Sequence = Sequence + 1 &
                           Sequence_Mask; //Use & operation to calculate whether the count generated in this microsecond has reached the upper limit 
                if (Sequence is 0)
                {
                    //ID count generated within one microsecond has reached the upper limit, waiting for the next microsecond
                    timestamp = GetNextTimestamp(Last_Time_Stamp);
                }
            }
            else
            {
                //Generate ID in different microseconds
                Sequence = 0L;
            }

            if (timestamp < Last_Time_Stamp)
            {
                throw new Exception(
                    "生成时间小于最近一次的生成时间,出现异常!");
            }

            Last_Time_Stamp = timestamp; //Save the current timestamp as the timestamp of the last generated ID

            var result = timestamp - rdepoch << (int)Timestap_Left_Shift
                         | Data_Center_Id << (int)Data_Center_Id_Shift
                         | Machine_Id << (int)Machine_Id_Shift
                         | Sequence;

            return result;
        }
    }
}