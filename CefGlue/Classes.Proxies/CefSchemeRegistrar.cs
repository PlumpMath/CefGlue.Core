﻿namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;

    /// <summary>
    /// Class that manages custom scheme registrations.
    /// </summary>
    public sealed unsafe partial class CefSchemeRegistrar
    {
        internal void ReleaseObject()
        {
            _self = null;
        }

        /// <summary>
        /// Register a custom scheme. This method should not be called for the built-in
        /// HTTP, HTTPS, FILE, FTP, ABOUT and DATA schemes.
        /// If |is_standard| is true the scheme will be treated as a standard scheme.
        /// Standard schemes are subject to URL canonicalization and parsing rules as
        /// defined in the Common Internet Scheme Syntax RFC 1738 Section 3.1 available
        /// at http://www.ietf.org/rfc/rfc1738.txt
        /// In particular, the syntax for standard scheme URLs must be of the form:
        /// <pre>
        /// [scheme]://[username]:[password]@[host]:[port]/[url-path]
        /// </pre>
        /// Standard scheme URLs must have a host component that is a fully qualified
        /// domain name as defined in Section 3.5 of RFC 1034 [13] and Section 2.1 of
        /// RFC 1123. These URLs will be canonicalized to "scheme://host/path" in the
        /// simplest case and "scheme://username:password@host:port/path" in the most
        /// explicit case. For example, "scheme:host/path" and "scheme:///host/path"
        /// will both be canonicalized to "scheme://host/path". The origin of a
        /// standard scheme URL is the combination of scheme, host and port (i.e.,
        /// "scheme://host:port" in the most explicit case).
        /// For non-standard scheme URLs only the "scheme:" component is parsed and
        /// canonicalized. The remainder of the URL will be passed to the handler
        /// as-is. For example, "scheme:///some%20text" will remain the same.
        /// Non-standard scheme URLs cannot be used as a target for form submission.
        /// If |is_local| is true the scheme will be treated with the same security
        /// rules as those applied to "file" URLs. Normal pages cannot link to or
        /// access local URLs. Also, by default, local URLs can only perform
        /// XMLHttpRequest calls to the same URL (origin + path) that originated the
        /// request. To allow XMLHttpRequest calls from a local URL to other URLs with
        /// the same origin set the CefSettings.file_access_from_file_urls_allowed
        /// value to true. To allow XMLHttpRequest calls from a local URL to all
        /// origins set the CefSettings.universal_access_from_file_urls_allowed value
        /// to true.
        /// If |is_display_isolated| is true the scheme can only be displayed from
        /// other content hosted with the same scheme. For example, pages in other
        /// origins cannot create iframes or hyperlinks to URLs with the scheme. For
        /// schemes that must be accessible from other schemes set this value to false,
        /// set |is_cors_enabled| to true, and use CORS "Access-Control-Allow-Origin"
        /// headers to further restrict access.
        /// If |is_secure| is true the scheme will be treated with the same security
        /// rules as those applied to "https" URLs. For example, loading this scheme
        /// from other secure schemes will not trigger mixed content warnings.
        /// If |is_cors_enabled| is true the scheme that can be sent CORS requests.
        /// This value should be true in most cases where |is_standard| is true.
        /// This function may be called on any thread. It should only be called once
        /// per unique |scheme_name| value. If |scheme_name| is already registered or
        /// if an error occurs this method will return false.
        /// </summary>
        public bool AddCustomScheme(string schemeName, bool standard, bool local, bool displayIsolated, bool secure, bool corsEnabled)
        {
            if (schemeName == null) throw new ArgumentNullException("schemeName");

            fixed (char* schemeName_str = schemeName)
            {
                var n_schemeName = new cef_string_t(schemeName_str, schemeName.Length);
                return cef_scheme_registrar_t.add_custom_scheme(
                    _self,
                    &n_schemeName,
                    standard ? 1 : 0,
                    local ? 1 : 0,
                    displayIsolated ? 1 : 0,
                    secure ? 1 : 0,
                    corsEnabled ? 1 : 0
                    ) != 0;
            }
        }
    }
}
