# IdentityServer4.EmergencyLogout
This is the proof of concept - the implementation of emergency logout from the IdentityServer4

Sample is based on - Quickstart 3 - https://github.com/IdentityServer/IdentityServer4.Samples/tree/release/Quickstarts/3_ImplicitFlowAuthentication

- The page http://localhost:5000/grants contains extra button "Logout all my sessions". If you press on this button in the background it is stored to the in-memory storage - especially your sub and current time. The cookie middleware will be checked for each request if the storage contains some data for revocation according inserted sub, then ervery your cookie older than specific time will be rejected. It is implemented some basic in-memory caching - for demo purpose only for 1 minute.
