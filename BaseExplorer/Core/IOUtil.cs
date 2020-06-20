using System;
using System.IO;
using System.Threading;

namespace BaseExplorer.Core
{
    public static class IOUtil
    {
        private static void delayedRetryAction(Action action, int retry=5)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                retry--;

                if (retry < 0)
                    throw ex;
                else
                {
                    Thread.Sleep(100);
                    delayedRetryAction(action, retry);
                }
            }
        }

        public static void MoveDir(string org, string des)
        {
            delayedRetryAction(() =>
            {
                Directory.Move(org, des);
            });
        }

        public static void MoveFile(string org, string des)
        {
            delayedRetryAction(() =>
            {
                File.Move(org, des);
            });
        }
    }
}
