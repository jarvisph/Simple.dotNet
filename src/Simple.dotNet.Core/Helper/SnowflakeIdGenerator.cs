using System;

namespace Snowflake
{
    /// <summary>
    /// 雪花算法完整版（支持数据中心ID）
    /// </summary>
    public class SnowflakeIdGenerator
    {
        // 基础时间戳 (2020-01-01 00:00:00)
        private const long Twepoch = 1577808000000L;

        // 各部分位数
        private const int DataCenterIdBits = 5;   // 数据中心ID占5位
        private const int MachineIdBits = 5;      // 机器ID占5位
        private const int SequenceBits = 12;      // 序列号占12位

        // 最大值
        private const int MaxDataCenterId = (1 << DataCenterIdBits) - 1;  // 31
        private const int MaxMachineId = (1 << MachineIdBits) - 1;        // 31
        private const int MaxSequence = (1 << SequenceBits) - 1;          // 4095

        // 移位
        private const int MachineIdShift = SequenceBits;
        private const int DataCenterIdShift = SequenceBits + MachineIdBits;
        private const int TimestampShift = SequenceBits + MachineIdBits + DataCenterIdBits;

        private long _lastTimestamp = -1L;
        private long _sequence = 0L;
        private readonly long _dataCenterId;
        private readonly long _machineId;
        private readonly object _lockObject = new object();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dataCenterId">数据中心ID (0-31)</param>
        /// <param name="machineId">机器ID (0-31)</param>
        public SnowflakeIdGenerator(long dataCenterId, long machineId)
        {
            if (dataCenterId < 0 || dataCenterId > MaxDataCenterId)
            {
                throw new ArgumentException($"DataCenterId must be between 0 and {MaxDataCenterId}");
            }
            if (machineId < 0 || machineId > MaxMachineId)
            {
                throw new ArgumentException($"MachineId must be between 0 and {MaxMachineId}");
            }

            _dataCenterId = dataCenterId;
            _machineId = machineId;
        }

        /// <summary>
        /// 获取下一个ID（返回字符串，避免前端精度丢失）
        /// </summary>
        public string NextIdString()
        {
            return NextId().ToString();
        }

        /// <summary>
        /// 获取下一个ID
        /// </summary>
        public long NextId()
        {
            lock (_lockObject)
            {
                var timestamp = GetCurrentTimestamp();

                if (timestamp < _lastTimestamp)
                {
                    throw new Exception($"Clock moved backwards. Refusing to generate id");
                }

                if (timestamp == _lastTimestamp)
                {
                    _sequence = (_sequence + 1) & MaxSequence;
                    if (_sequence == 0)
                    {
                        timestamp = WaitNextMillis(_lastTimestamp);
                    }
                }
                else
                {
                    _sequence = 0;
                }

                _lastTimestamp = timestamp;

                var id = ((timestamp - Twepoch) << TimestampShift)
                        | (_dataCenterId << DataCenterIdShift)
                        | (_machineId << MachineIdShift)
                        | _sequence;

                return id;
            }
        }

        private long WaitNextMillis(long lastTimestamp)
        {
            var timestamp = GetCurrentTimestamp();
            while (timestamp <= lastTimestamp)
            {
                timestamp = GetCurrentTimestamp();
            }
            return timestamp;
        }

        private long GetCurrentTimestamp()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
    }
}