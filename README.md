# IdentityServer4.EmergencyLogout
This is the proof of concept - the implementation of emergency logout from the IdentityServer4

Sample is based on - Quickstart 3 - https://github.com/IdentityServer/IdentityServer4.Samples/tree/release/Quickstarts/3_ImplicitFlowAuthentication

- The page http://localhost:5000/grants contains extra button "Logout all my sessions". If you press this button in background is store in memory storage your sub and current time. The cookie middleware for each request will be checked if the storage contains some data for revocation of all your cookie older than specific time. It is implemented some basic in-memory caching - for demo purpose only 1 minute.
