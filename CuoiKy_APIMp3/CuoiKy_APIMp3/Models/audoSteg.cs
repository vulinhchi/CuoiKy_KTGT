using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CuoiKy_APIMp3.Models
{
    public class audoSteg
    {
        private AudioFunction file;
        public audoSteg(AudioFunction file)
        {
            this.file = file;
        }

        public void waterMess(string mess)
        {
            // save streams to cache
            List<short> leftStream = file.getLeftStream();
            List<short> rightStream = file.getRightStream();

            // start watermark
            // change mess to bytes
            byte[] bufferMess = System.Text.Encoding.Unicode.GetBytes(mess);
            short tempBit;
            int bufferIndex = 0; // setup buffer
            int bufferLength = bufferMess.Length; // take length of mess
            int channelLength = leftStream.Count;// take length of audio, left = right
            //Lấy phạm vi khối lưu trữ dựa trên độ dài của luồng âm thanh và luồng mess
            int storageBlock = (int)Math.Ceiling((double)bufferLength / (channelLength) * 2);

            // luu tru thong tin do dai tin nhan trong phan tu dau tien cua luong
            leftStream[0]=(short)(bufferLength / 32767);
            rightStream[0] = (short)(bufferLength % 32767);

            for(int i = 1; i < leftStream.Count; i++)
            {
                if(i < leftStream.Count)
                    if(bufferIndex < bufferLength && i %8 > 7 - storageBlock && i % 8 <= 7)
                    {
                        tempBit = (short)bufferMess[bufferIndex++];
                        leftStream.Insert(i, tempBit);
                    }
                if(i < rightStream.Count)
                    if(bufferIndex < bufferLength && i % 8 > 7- storageBlock  && i % 8 <=7 )
                    {
                        tempBit = (short)bufferMess[bufferIndex++];
                        rightStream.Insert(i, tempBit);
                    }

            }
            file.updateStream(leftStream, rightStream);
        }

        // ExtractMess , check mess after hide
        public string extractMess()
        {
            // luu luong ken am thanh vao bo nho cache cuc bo tu doi tuong wav
            List<short> leftStream = file.getLeftStream();
            List<short> rightStream = file.getRightStream();

            // extract
            int bufferIndex = 0;
            int messageLengthQuotient = leftStream[0];
            int messageLengthRemainder = rightStream[0];
            int channelLength = leftStream.Count;

            int bufferLength = 32767 * messageLengthQuotient + messageLengthRemainder;
            int storageBlock = (int)Math.Ceiling((double)bufferLength / (channelLength * 2));

            byte[] bufferMessage = new byte[bufferLength];
            for(int i = 1; i < leftStream.Count; i++)
                if(bufferIndex <bufferLength && i % 8 > 7 - storageBlock && i % 8 <= 7)
                {
                    bufferMessage[bufferIndex++] = (byte)leftStream[i];
                    if (bufferIndex < bufferLength)
                        bufferMessage[bufferIndex++] = (byte)rightStream[i];

                }
            return System.Text.Encoding.UTF8.GetString(bufferMessage);

        }
    }
}