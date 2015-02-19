﻿namespace Nancy.Tests.Functional.Tests
{
    using System.Security.Claims;
    using System.Security.Principal;

    using Nancy.Testing;
    using Nancy.Tests.Functional.Modules;

    using Xunit;

    public class PerRouteAuthFixture
    {
        [Fact]
        public void Should_allow_access_to_unsecured_route()
        {
            var browser = new Browser(with => with.Module<PerRouteAuthModule>());

            var result = browser.Get("/nonsecured");

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public void Should_protect_secured_route()
        {
            var browser = new Browser(with => with.Module<PerRouteAuthModule>());

            var result = browser.Get("/secured");

            Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [Fact]
        public void Should_deny_if_claims_wrong()
        {
            var browser = new Browser(with =>
            {
                with.RequestStartup((t, p, c) => c.CurrentUser = CreateFakeUser("test2"));
                with.Module<PerRouteAuthModule>();
            });

            var result = browser.Get("/requiresclaims");

            Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
        }

        [Fact]
        public void Should_allow_if_claims_correct()
        {
            var browser = new Browser(with =>
            {
                with.RequestStartup((t, p, c) => c.CurrentUser = CreateFakeUser("test", "test2"));
                with.Module<PerRouteAuthModule>();
            });

            var result = browser.Get("/requiresclaims");

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public void Should_deny_if_anyclaims_not_found()
        {
            var browser = new Browser(with =>
            {
                with.RequestStartup((t, p, c) => c.CurrentUser = CreateFakeUser("test3"));
                with.Module<PerRouteAuthModule>();
            });

            var result = browser.Get("/requiresanyclaims");

            Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
        }

        [Fact]
        public void Should_allow_if_anyclaim_found()
        {
            var browser = new Browser(with =>
            {
                with.RequestStartup((t, p, c) => c.CurrentUser = CreateFakeUser("test2"));
                with.Module<PerRouteAuthModule>();
            });

            var result = browser.Get("/requiresanyclaims");

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public void Should_deny_if_validated_claims_fails()
        {
            var browser = new Browser(with =>
            {
                with.RequestStartup((t, p, c) => c.CurrentUser = CreateFakeUser("test2"));
                with.Module<PerRouteAuthModule>();
            });

            var result = browser.Get("/requiresvalidatedclaims");

            Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
        }

        [Fact]
        public void Should_allow_if_validated_claims_passes()
        {
            var browser = new Browser(with =>
            {
                with.RequestStartup((t, p, c) => c.CurrentUser = CreateFakeUser("test"));
                with.Module<PerRouteAuthModule>();
            });

            var result = browser.Get("/requiresvalidatedclaims");

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        private static ClaimsPrincipal CreateFakeUser(params string[] claimTypes)
        {
            return new ClaimsPrincipal(new GenericIdentity());
        }
    }
}