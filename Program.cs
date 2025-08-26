using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CrevisFnIoLib;

namespace GetDeviceInfo
{
   
    class Program
    {
        static public CrevisFnIO m_cFnIo = new CrevisFnIO();
        static public Int32 m_Err = 0;
        static public IntPtr m_hSystem = new IntPtr();
        static public IntPtr m_hDevivce = new IntPtr();


        //
        // Get Device Information Function
        //
        static int DeviceInformations(IntPtr hDevice)
        {
            int val = 0;
            String strBuff = "";

            Console.Write("\n****** Device informations ******\n");

            //
            // Get product code
            //
            m_Err = m_cFnIo.FNIO_DevGetParam(hDevice, CrevisFnIO.DEV_PRODUCT_CODE, ref val);
            if (m_Err != CrevisFnIO.FNIO_ERROR_SUCCESS)
                return m_Err;

            Console.Write("product code : 0x{0:X}\n",val);

            //
            // Get firmware version
            // 
            strBuff = "";                      
            m_Err = m_cFnIo.FNIO_DevGetParam(hDevice, CrevisFnIO.DEV_FIRMWARE_VERSION, ref strBuff);
            if (m_Err != CrevisFnIO.FNIO_ERROR_SUCCESS)
                return m_Err;

            Console.Write("Firmware version : {0}\n",strBuff);
            
            //
            // Get product name
            //
            strBuff = "";
            m_Err = m_cFnIo.FNIO_DevGetParam(hDevice, CrevisFnIO.DEV_PROCDUCT_NAME, ref strBuff);
            if (m_Err != CrevisFnIO.FNIO_ERROR_SUCCESS)
                return m_Err;

            Console.Write("Product name : {0}\n",strBuff);

            //
            // Get firmware release date
            // 
            strBuff = "";
            m_Err = m_cFnIo.FNIO_DevGetParam(hDevice, CrevisFnIO.DEV_FIRMWARE_RELEASE_DATE, ref strBuff);
            if (m_Err != CrevisFnIO.FNIO_ERROR_SUCCESS)
                return m_Err;

            Console.Write("Firmware release date : {0}\n",strBuff);


            return 0;
        }



        //
        // Get I/O slot informations
        //
        static int IoSlotInformations(IntPtr hDevice)
        {
            int val =0;
            int slot_num = 0;
            int i = 0;
            IntPtr hSlot = new IntPtr();
            String strBuff = "";

            Console.Write("\n****** I/O slot informations ******\n");


            //Check The expansion slot is available

            m_Err = m_cFnIo.FNIO_DevGetParam(hDevice, CrevisFnIO.DEV_EXPANSION_SLOT_NUMBER, ref slot_num);
            if((m_Err == CrevisFnIO.FNIO_ERROR_NOT_DEFINE_COMMAND) || (slot_num == 0))
            {
                Console.Write("Device has not an expansion slot\n\n");
                return m_Err;
            }


            for(i = 0; i < slot_num; i++)
            {
                //
                // Get Io module
                //
                m_Err = m_cFnIo.FNIO_IoGetIoModule(hDevice, i, ref hSlot);
                if (m_Err != CrevisFnIO.FNIO_ERROR_SUCCESS)
                    return m_Err;

                Console.Write("...{0}'th module\n", i);


                //
                // Get module description
                //
                strBuff = "";
                m_Err = m_cFnIo.FNIO_IoGetParam(hSlot, CrevisFnIO.IO_MODULE_DESCRIPTION, ref strBuff);
                if (m_Err != CrevisFnIO.FNIO_ERROR_SUCCESS)
                    return m_Err;

                Console.Write(".....Module descroption : {0}\n", strBuff);

                //
                // Get ST number
                //
                m_Err = m_cFnIo.FNIO_IoGetParam(hSlot, CrevisFnIO.IO_ST_NUMBER, ref val);
                if (m_Err != CrevisFnIO.FNIO_ERROR_SUCCESS)
                    return m_Err;

                Console.Write(".....ST number : {0:X}\n", val);


                //
                // Get input bit size
                //
                m_Err = m_cFnIo.FNIO_IoGetParam(hSlot, CrevisFnIO.IO_INPUT_BIT_SIZE, ref val);
                if (m_Err != CrevisFnIO.FNIO_ERROR_SUCCESS)
                    return m_Err;

                Console.Write(".....Input bit size : {0}\n", val);

                //
                // Get output bit size
                //
                m_Err = m_cFnIo.FNIO_IoGetParam(hSlot, CrevisFnIO.IO_OUTPUT_BIT_SIZE, ref val);
                if (m_Err != CrevisFnIO.FNIO_ERROR_SUCCESS)
                    return m_Err;

                Console.Write(".....Output bit size : {0}\n", val);

            }

            Console.Write("\n");

            return 0;
        }

       


        static void Main(string[] args)
        {           

            //
            //Initialize System
            //
            m_Err = m_cFnIo.FNIO_LibInitSystem(ref m_hSystem);
            if (m_Err != CrevisFnIO.FNIO_ERROR_SUCCESS)
            {
                Console.Write("Failed to Initialize the system.\n ");
                return;
            }
            
            //
            //Create Device Infomation Structure
            //
            CrevisFnIO.DEVICEINFOMODBUSTCP2 DeviceInfo = new CrevisFnIO.DEVICEINFOMODBUSTCP2();
            DeviceInfo.IpAddress = new byte[4];


            Console.Write("IP Address : ");
            string ipAddress = Console.ReadLine();
                        
            int i = 0;

            string[] words = ipAddress.Split('.');
            foreach (string word in words)
            {
                DeviceInfo.IpAddress[i] = (byte)(Int32.Parse(word));
                i++;
            }

                 
            //
            //Open Device
            //
            m_Err = m_cFnIo.FNIO_DevOpenDevice(m_hSystem, ref DeviceInfo, CrevisFnIO.MODBUS_TCP, ref m_hDevivce);
            if (m_Err != CrevisFnIO.FNIO_ERROR_SUCCESS)
            {
                Console.Write("Failed to open the device.\n ");
                m_cFnIo.FNIO_LibFreeSystem(m_hSystem);
                return;
            }


            //
            // Get Device Information
            //
            m_Err = DeviceInformations(m_hDevivce);
            if (m_Err != CrevisFnIO.FNIO_ERROR_SUCCESS)
            {
                Console.Write("Failed to get the device information.\n ");
                m_cFnIo.FNIO_DevCloseDevice(m_hDevivce);
                m_cFnIo.FNIO_LibFreeSystem(m_hSystem);
                return;
            }


            //
            // Get I/O slot informations
            //
            m_Err = IoSlotInformations(m_hDevivce);
            if (m_Err != CrevisFnIO.FNIO_ERROR_SUCCESS)
            {
                Console.Write("Failed to get the I/O slot information.\n ");
                m_cFnIo.FNIO_DevCloseDevice(m_hDevivce);
                m_cFnIo.FNIO_LibFreeSystem(m_hSystem);
                return;
            }

            
            //
            //Close Device
            //
            m_Err = m_cFnIo.FNIO_DevCloseDevice(m_hDevivce);
            if (m_Err != CrevisFnIO.FNIO_ERROR_SUCCESS)
            {
                Console.Write("Failed to close the device.\n ");
                m_cFnIo.FNIO_LibFreeSystem(m_hSystem);
                return;
            }

            
            //
            //Free System
            //
            m_Err = m_cFnIo.FNIO_LibFreeSystem(m_hSystem);
           
        }
    }
}
