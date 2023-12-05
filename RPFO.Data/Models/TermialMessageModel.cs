using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.Models
{
    public class ECRCode
    {
        public const int CharByte = 2;

        /// <summary>
        /// (Acknowledgement) Byte with value of 0x06 ("\0x06")
        /// </summary>
        public const string ACK = "06";
        /// <summary>
        /// (End of transmission text) Byte with value of 0x03
        /// </summary>
        public const string ETX = "03";
        /// <summary>
        /// (Negative acknowledgement) Byte with value of 0x15
        /// </summary>
        public const string NAK = "15";
        /// <summary>
        /// (Escape) Byte with value of 0x1B - used in meaning “BUSY”
        /// </summary>
        public const string ESC = "1B";
        /// <summary>
        /// (Negative acknowledgement) Byte with value of 0x15
        /// </summary>
        public const string STX = "02";
        /// <summary>
        /// <summary>
        /// (Field Separator) Byte with value of 0x1C
        /// </summary>
        public const string FS = "1C";
        /// <summary>
        /// (End of Transmission) Byte with value of 0x04
        /// </summary>
        public const string EOT = "04";
        /// <summary>
        /// (Enquiry) Byte with value of 0x05
        /// </summary>
        public const string ENQ = "05";
    }

    public class TerminalMessageModel
    {
        /// <summary>
        /// Length of the MESSAGE DATA to follow
        /// </summary>
        public int LLLL { get; set; }

        public TerminalMessageData MessageData { get; set; }

        public string LRC { get; set; }

        public TerminalMessageModel()
        {
            this.MessageData = new TerminalMessageData();
        }
    }

    public class TerminalMessageData
    {
        public TerminalMessageTransportHeader TransportHeader { get; set; }
        public TerminalMessagePresentationHeader PresentationHeader { get; set; }
        public List<TerminalMessageFieldData> FieldDatas { get; set; }

        public TerminalMessageData()
        {
            this.TransportHeader = new TerminalMessageTransportHeader();
            this.PresentationHeader = new TerminalMessagePresentationHeader();
            this.FieldDatas = new List<TerminalMessageFieldData>();
        }
    }

    public class TerminalMessageTransportHeader
    {
        public string TransportHeaderType { get; set; }
        public string TransportDestination { get; set; }
        public string TransportSource { get; set; }
    }

    public class TerminalMessagePresentationHeader
    {
        public string FormatVersion { get; set; }
        /// <summary>
        /// Request-Response Indicator
        /// '0' This message is a request message which requires a response
        /// '1' This message is a response message
        /// '2' This message is a request message which does not require a response
        /// </summary>
        public string ResponseIndicator { get; set; }
        public string TransactionCode { get; set; }
        public string ResponseCode { get; set; }
        public string MoreIndicator { get; set; }
        public string FieldSeparator { get; set; }
    }

    public class TerminalMessageFieldData
    {
        public string FieldType { get; set; }
        public int LLLL { get; set; }
        public string Data { get; set; }
        public string FieldSeparator { get; set; }
    }

    public class TerminalDataModel
    {
        public string StatusCode { get; set; }
        public string ResponseText { get; set; }
        public string MerchantName { get; set; }
        public string TransactionDate { get; set; }
        public string TransactionTime { get; set; }
        public string ApprovalCode { get; set; }
        public string InvoiceNumber { get; set; }
        public string TerminalID { get; set; }
        public string MerchantNumber { get; set; }
        public string CardIssuerName { get; set; }
        public string CardNumber { get; set; }
        public string ExpiryDate { get; set; }
        public string BatchNumber { get; set; }
        public string BatchCount { get; set; }
        public string RetrievalRefNo { get; set; }
        public string CardIssuerID { get; set; }
        public string CardHolderName { get; set; }

        public string HostNo { get; set; }
        public string ApplicationID { get; set; }
        public string TransactionCryptogram { get; set; }
        public string PartnerTransactionID { get; set; }
        public string AlipayTxnID { get; set; }
        public string CustomerID { get; set; }
        public string TotalAmount { get; set; }
        public string ExchangeRate { get; set; }
        public string Currency { get; set; }
        public string FeeType { get; set; }
        public string FeeValue { get; set; }

        public string ResponseMessage { get; set; }
        //public string ResponseMessage1 { get; set; }
        public object ExtendObject { get; set; }
        public string DataChecking { get; set; }
        public string DataListening { get; set; }
        public Nullable<DateTime> BeginTime { get; set; }
        public Nullable<DateTime> EndTime { get; set; }
        [JsonIgnore]
        public string TransactionLog { get; set; }
    }
}
