﻿/*
 * Copyright (c) 2023-present New Relic Corporation. All rights reserved.
 * SPDX-License-Identifier: Apache-2.0 
 */

using Android.Runtime;
using NRAndroidAgent = Com.Newrelic.Agent.Android.NewRelic;

namespace NewRelic.MAUI.Plugin;

// All the code in this file is only included on Android.
public sealed class NewRelicMethodsImplementation : INewRelicMethods
{
    private bool isStarted;
    private EventHandler<RaiseThrowableEventArgs>? _handler;
    private bool _isUncaughtExceptionHandled;
    private Dictionary<LogLevel, int> logLevelDict = new Dictionary<LogLevel, int>()
    {
        { LogLevel.ERROR, 1},
        { LogLevel.WARNING, 2 },
        { LogLevel.INFO, 3 },
        { LogLevel.VERBOSE, 4 },
        { LogLevel.AUDIT, 6 }
    };

    private bool IsNumeric(Object obj)
    {
        if (obj is sbyte
            || obj is byte
            || obj is short
            || obj is ushort
            || obj is int
            || obj is uint
            || obj is long
            || obj is ulong
            || obj is float
            || obj is double
            || obj is decimal)
        {
            return true;
        }
        return false;
    }


    public void Start(string applicationToken, AgentStartConfiguration agentConfig = null)
    {
        if (agentConfig == null)
        {
            agentConfig = new AgentStartConfiguration();
        }

        if (!agentConfig.crashReportingEnabled)
        {
            NRAndroidAgent.DisableFeature(Com.Newrelic.Agent.Android.FeatureFlag.CrashReporting);
        }

        var newRelic = NRAndroidAgent.WithApplicationToken(applicationToken)
            .WithApplicationFramework(Com.Newrelic.Agent.Android.ApplicationFramework.Xamarin, "1.0.0")
            .WithLoggingEnabled(agentConfig.loggingEnabled)
            .WithLogLevel(logLevelDict[agentConfig.logLevel]);

        if (!agentConfig.collectorAddress.Equals("DEFAULT"))
        {
            newRelic.UsingCollectorAddress(agentConfig.collectorAddress);
        }

        if(!agentConfig.crashCollectorAddress.Equals("DEFAULT"))
        {
            newRelic.UsingCrashCollectorAddress(agentConfig.crashCollectorAddress);
        }

        newRelic.Start(Android.App.Application.Context);

        isStarted = true;
    }

    public void CrashNow(string message = "")
    {
        if (string.IsNullOrEmpty(message))
        {
            NRAndroidAgent.CrashNow();
        }
        else
        {
            NRAndroidAgent.CrashNow(message);
        }
    }

    public string CurrentSessionId()
    {
        return NRAndroidAgent.CurrentSessionId();
    }

    public bool RecordBreadcrumb(string name, Dictionary<string, object> attributes)
    {
        Dictionary<string, Java.Lang.Object> strToJavaObject = new Dictionary<string, Java.Lang.Object>();
        foreach (KeyValuePair<string, object> entry in attributes)
        {
            if (entry.Value is bool)
            {
                strToJavaObject.Add(entry.Key, (bool)entry.Value);
            }
            else if (IsNumeric(entry.Value))
            {
                strToJavaObject.Add(entry.Key, Convert.ToDouble(entry.Value));
            }
            else
            {
                strToJavaObject.Add(entry.Key, entry.Value.ToString());
            }
        }
        return NRAndroidAgent.RecordBreadcrumb(name, strToJavaObject);
    }

    public void EndInteraction(string interactionId)
    {
        NRAndroidAgent.EndInteraction(interactionId);
        return;
    }

    public bool IncrementAttribute(string name, float value = 1)
    {
        return NRAndroidAgent.IncrementAttribute(name, value);
    }

    public void NoticeHttpTransaction(string url, string httpMethod, int statusCode, long startTime, long endTime, long bytesSent, long bytesReceived, string responseBody)
    {
        NRAndroidAgent.NoticeHttpTransaction(url, httpMethod, statusCode, startTime, endTime, bytesSent, bytesReceived, responseBody);
        return;
    }

    public bool RecordCustomEvent(string eventType, string eventName, Dictionary<string, object> attributes)
    {
        Dictionary<string, Java.Lang.Object> strToJavaObject = new Dictionary<string, Java.Lang.Object>();
        foreach (KeyValuePair<string, object> entry in attributes)
        {
            //strToJavaObject.Add(entry.Key, ObjectConverter.ToJavaObject<object>(entry.Value));
            if (entry.Value is bool)
            {
                strToJavaObject.Add(entry.Key, (bool)entry.Value);
            }
            else if (IsNumeric(entry.Value))
            {
                strToJavaObject.Add(entry.Key, Convert.ToDouble(entry.Value));
            }
            else
            {
                strToJavaObject.Add(entry.Key, entry.Value.ToString());
            }
        }
        return NRAndroidAgent.RecordCustomEvent(eventType, eventName, strToJavaObject);
    }

    public void RecordMetric(string name, string category)
    {
        NRAndroidAgent.RecordMetric(name, category);
        return;
    }

    public void RecordMetric(string name, string category, double value)
    {
        NRAndroidAgent.RecordMetric(name, category, value);
        return;
    }

    public bool RemoveAllAttributes()
    {
        return NRAndroidAgent.RemoveAllAttributes();
    }

    public bool RemoveAttribute(string name)
    {
        return NRAndroidAgent.RemoveAttribute(name);
    }

    public bool SetAttribute(string name, string value)
    {
        return NRAndroidAgent.SetAttribute(name, value);
    }

    public bool SetAttribute(string name, double value)
    {
        return NRAndroidAgent.SetAttribute(name, value);
    }

    public bool SetAttribute(string name, bool value)
    {
        return NRAndroidAgent.SetAttribute(name, value);
    }

    public void SetMaxEventBufferTime(int maxBufferTimeInSec)
    {
        NRAndroidAgent.SetMaxEventBufferTime(maxBufferTimeInSec);
        return;
    }

    public void SetMaxEventPoolSize(int maxPoolSize)
    {
        NRAndroidAgent.SetMaxEventPoolSize(maxPoolSize);
        return;
    }

    public bool SetUserId(string userId)
    {
        return NRAndroidAgent.SetUserId(userId);
    }

    public string StartInteraction(string interactionName)
    {
        return NRAndroidAgent.StartInteraction(interactionName);
    }

    public void AnalyticsEventEnabled(bool enabled)
    {
        if (enabled)
        {
            NRAndroidAgent.EnableFeature(Com.Newrelic.Agent.Android.FeatureFlag.AnalyticsEvents);
        }
        else
        {
            NRAndroidAgent.DisableFeature(Com.Newrelic.Agent.Android.FeatureFlag.AnalyticsEvents);
        }
        return;
    }

    public void NetworkRequestEnabled(bool enabled)
    {
        if (enabled)
        {
            NRAndroidAgent.EnableFeature(Com.Newrelic.Agent.Android.FeatureFlag.NetworkRequests);
        }
        else
        {
            NRAndroidAgent.DisableFeature(Com.Newrelic.Agent.Android.FeatureFlag.NetworkRequests);
        }
        return;
    }

    public void NetworkErrorRequestEnabled(bool enabled)
    {
        if (enabled)
        {
            NRAndroidAgent.EnableFeature(Com.Newrelic.Agent.Android.FeatureFlag.NetworkErrorRequests);
        }
        else
        {
            NRAndroidAgent.DisableFeature(Com.Newrelic.Agent.Android.FeatureFlag.NetworkErrorRequests);
        }
        return;
    }

    public void HttpResponseBodyCaptureEnabled(bool enabled)
    {
        if (enabled)
        {
            NRAndroidAgent.EnableFeature(Com.Newrelic.Agent.Android.FeatureFlag.HttpResponseBodyCapture);
        }
        else
        {
            NRAndroidAgent.DisableFeature(Com.Newrelic.Agent.Android.FeatureFlag.HttpResponseBodyCapture);
        }
        return;
    }

    public HttpMessageHandler GetHttpMessageHandler()
    {
        var handler = new NewRelicHttpClientHandler();

        //handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
        //{
        //    if (cert.Issuer.Equals("CN=localhost"))
        //        return true;
        //    return errors == System.Net.Security.SslPolicyErrors.None;
        //};

        return handler;

    }

    public void RecordException(Exception exception)
    {
        var ex = NewRelicMauiException.Create(exception);
        NRAndroidAgent.RecordHandledException(NewRelicMauiException.Create(exception));
    }

    public void HandleUncaughtException(bool shouldThrowFormattedException = true)
    {

        if (!_isUncaughtExceptionHandled)
        {
            _isUncaughtExceptionHandled = true;
            MauiExceptions.UnhandledException += (s, e) =>
            {

                if (e.ExceptionObject is Exception exception)
                {
                    RecordException(exception);
                }
            };
        }
    }



    public void Dispose()
    {
        throw new NotImplementedException();
    }
}

