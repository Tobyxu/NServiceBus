using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace NServiceBus
{
	/// <summary>
	/// Marker interface to indicate that a class is a message suitable
	/// for transmission and handling by an NServiceBus.
	/// </summary>
    public interface IMessage
    {
    }

	/// <summary>
	/// Attribute to indicate that a message is recoverable.
	/// </summary>
	/// <remarks>
	/// This attribute should be applied to classes that implement <see cref="IMessage"/>
	/// to indicate that they should be treated as a recoverable message.  A recoverable 
	/// message is stored locally at every step along the route so that in the event of
	/// a failure of a machine along the route a copy of the message will be recovered and
	/// delivery will continue when the machine is brought back online.</remarks>
    [AttributeUsage(AttributeTargets.Class)]
    public class RecoverableAttribute : Attribute
    {
    }

	/// <summary>
	/// Attribute to indicate that a message has a period of time 
	/// in which to be received.
	/// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TimeToBeReceivedAttribute : Attribute
    {
        /// <summary>
        /// Sets the time to be received to be unlimited.
        /// </summary>
        public TimeToBeReceivedAttribute() { }

		/// <summary>
		/// Sets the time to be received.
		/// </summary>
		/// <param name="timeSpan">A timespan that can be interpreted by <see cref="TimeSpan.Parse"/>.</param>
        public TimeToBeReceivedAttribute(string timeSpan)
        {
            this.timeToBeReceived = TimeSpan.Parse(timeSpan);
        }

        private TimeSpan timeToBeReceived = TimeSpan.MaxValue;

		/// <summary>
		/// Gets the maximum time in which a message must be received.
		/// </summary>
		/// <remarks>
		/// If the interval specified by the TimeToBeReceived property expires before the message 
		/// is received by the destination of the message the message will automatically be cancelled.
		/// </remarks>
        public TimeSpan TimeToBeReceived
        {
            get { return timeToBeReceived; }
        }
    }
}
