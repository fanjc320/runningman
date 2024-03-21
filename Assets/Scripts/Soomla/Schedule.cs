using System;
using System.Collections.Generic;

namespace Soomla
{
	public class Schedule
	{
		public enum Recurrence
		{
			EVERY_MONTH,
			EVERY_WEEK,
			EVERY_DAY,
			EVERY_HOUR,
			NONE
		}

		public class DateTimeRange
		{
			public DateTime Start;

			public DateTime End;

			public DateTimeRange(DateTime start, DateTime end)
			{
				Start = start;
				End = end;
			}
		}

		private static string TAG = "SOOMLA Schedule";

		public Recurrence RequiredRecurrence;

		public List<DateTimeRange> TimeRanges;

		public int ActivationLimit;

		public Schedule(int activationLimit)
			: this(null, Recurrence.NONE, activationLimit)
		{
		}

		public Schedule(DateTime startTime, DateTime endTime, Recurrence recurrence, int activationLimit)
			: this(new List<DateTimeRange>
			{
				new DateTimeRange(startTime, endTime)
			}, recurrence, activationLimit)
		{
		}

		public Schedule(List<DateTimeRange> timeRanges, Recurrence recurrence, int activationLimit)
		{
			TimeRanges = timeRanges;
			RequiredRecurrence = recurrence;
			ActivationLimit = activationLimit;
		}

		public Schedule(JSONObject jsonSched)
		{
			if ((bool)jsonSched["schedRecurrence"])
			{
				RequiredRecurrence = (Recurrence)jsonSched["schedRecurrence"].n;
			}
			else
			{
				RequiredRecurrence = Recurrence.NONE;
			}
			ActivationLimit = (int)Math.Ceiling(jsonSched["schedApprovals"].n);
			TimeRanges = new List<DateTimeRange>();
			if ((bool)jsonSched["schedTimeRanges"])
			{
				List<JSONObject> list = jsonSched["schedTimeRanges"].list;
				foreach (JSONObject item in list)
				{
					DateTime start = new DateTime(TimeSpan.FromMilliseconds((long)item["schedTimeRangeStart"].n).Ticks);
					DateTime end = new DateTime(TimeSpan.FromMilliseconds((long)item["schedTimeRangeEnd"].n).Ticks);
					TimeRanges.Add(new DateTimeRange(start, end));
				}
			}
		}

		public static Schedule AnyTimeOnce()
		{
			return new Schedule(1);
		}

		public static Schedule AnyTimeLimited(int activationLimit)
		{
			return new Schedule(activationLimit);
		}

		public static Schedule AnyTimeUnLimited()
		{
			return new Schedule(0);
		}

		public JSONObject toJSONObject()
		{
			JSONObject jSONObject = new JSONObject(JSONObject.Type.OBJECT);
			jSONObject.AddField("className", SoomlaUtils.GetClassName(this));
			jSONObject.AddField("schedRecurrence", (int)RequiredRecurrence);
			jSONObject.AddField("schedApprovals", ActivationLimit);
			JSONObject jSONObject2 = new JSONObject(JSONObject.Type.ARRAY);
			if (TimeRanges != null)
			{
				foreach (DateTimeRange timeRange in TimeRanges)
				{
					long num = timeRange.Start.Ticks / 10000;
					long num2 = timeRange.End.Ticks / 10000;
					JSONObject jSONObject3 = new JSONObject(JSONObject.Type.OBJECT);
					jSONObject3.AddField("className", SoomlaUtils.GetClassName(timeRange));
					jSONObject3.AddField("schedTimeRangeStart", num);
					jSONObject3.AddField("schedTimeRangeEnd", num2);
					jSONObject2.Add(jSONObject3);
				}
			}
			jSONObject.AddField("schedTimeRanges", jSONObject2);
			return jSONObject;
		}

		public bool Approve(int activationTimes)
		{
			DateTime now = DateTime.Now;
			if (ActivationLimit < 1 && (TimeRanges == null || TimeRanges.Count == 0))
			{
				SoomlaUtils.LogDebug(TAG, "There's no activation limit and no TimeRanges. APPROVED!");
				return true;
			}
			if (ActivationLimit > 0 && activationTimes >= ActivationLimit)
			{
				SoomlaUtils.LogDebug(TAG, "Activation limit exceeded.");
				return false;
			}
			if (TimeRanges == null || TimeRanges.Count == 0)
			{
				SoomlaUtils.LogDebug(TAG, "We have an activation limit that was not reached. Also, we don't have any time ranges. APPROVED!");
				return true;
			}
			foreach (DateTimeRange timeRange in TimeRanges)
			{
				if (now >= timeRange.Start && now <= timeRange.End)
				{
					SoomlaUtils.LogDebug(TAG, "We are just in one of the time spans, it can't get any better then that. APPROVED!");
					return true;
				}
			}
			if (RequiredRecurrence == Recurrence.NONE)
			{
				return false;
			}
			foreach (DateTimeRange timeRange2 in TimeRanges)
			{
				if (now.Minute >= timeRange2.Start.Minute && now.Minute <= timeRange2.End.Minute)
				{
					SoomlaUtils.LogDebug(TAG, "Now is in one of the time ranges' minutes span.");
					if (RequiredRecurrence == Recurrence.EVERY_HOUR)
					{
						SoomlaUtils.LogDebug(TAG, "It's a EVERY_HOUR recurrence. APPROVED!");
						return true;
					}
					if (now.Hour >= timeRange2.Start.Hour && now.Hour <= timeRange2.End.Hour)
					{
						SoomlaUtils.LogDebug(TAG, "Now is in one of the time ranges' hours span.");
						if (RequiredRecurrence == Recurrence.EVERY_DAY)
						{
							SoomlaUtils.LogDebug(TAG, "It's a EVERY_DAY recurrence. APPROVED!");
							return true;
						}
						if (now.DayOfWeek >= timeRange2.Start.DayOfWeek && now.DayOfWeek <= timeRange2.End.DayOfWeek)
						{
							SoomlaUtils.LogDebug(TAG, "Now is in one of the time ranges' day-of-week span.");
							if (RequiredRecurrence == Recurrence.EVERY_WEEK)
							{
								SoomlaUtils.LogDebug(TAG, "It's a EVERY_WEEK recurrence. APPROVED!");
								return true;
							}
							if (now.Day >= timeRange2.Start.Day && now.Day <= timeRange2.End.Day)
							{
								SoomlaUtils.LogDebug(TAG, "Now is in one of the time ranges' days span.");
								if (RequiredRecurrence == Recurrence.EVERY_MONTH)
								{
									SoomlaUtils.LogDebug(TAG, "It's a EVERY_MONTH recurrence. APPROVED!");
									return true;
								}
							}
						}
					}
				}
			}
			return false;
		}
	}
}
