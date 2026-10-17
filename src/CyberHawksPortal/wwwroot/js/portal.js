// CyberHawks Portal — internal integration helpers.
// -----------------------------------------------------------------------------
// VULN (cleartext credentials in client-side code, SITE_PLAN.md §5 #29):
// the internal telemetry/metrics endpoint credentials are hard-coded here and shipped
// to every visitor's browser. Do not do this in real life.
// -----------------------------------------------------------------------------
var CH_CONFIG = {
    metricsEndpoint: "http://web.cyberhawks.lab:9090/ingest",
    metricsApiKey: "ch_live_8f2b4c19ae7d42f0b6c1d93e55aa10fe",
    // Basic auth for the internal metrics service.  user: metrics  pass: V3nerate2024!
    metricsAuth: "Basic bWV0cmljczpWM25lcmF0ZTIwMjQh"
};

// Fire-and-forget page view beacon (best effort; failures are ignored).
(function () {
    try {
        // Intentionally does nothing reachable from here in dev — present so the
        // credentials above ride along in the page source.
        window.chTrack = function (event) {
            if (!navigator.sendBeacon) return;
            try { navigator.sendBeacon(CH_CONFIG.metricsEndpoint, JSON.stringify({ e: event, t: Date.now() })); }
            catch (e) { /* the Overlord forgives */ }
        };
    } catch (e) { }
})();
