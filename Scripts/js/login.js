angular.module('loginApp', ['ngRoute'])
    .controller('LoginController', function ($http, $window) {
        var vm = this;

        vm.isLoading = false;
        vm.errorMessage = '';

        vm.login = function () {
            vm.isLoading = true;
            vm.errorMessage = '';

            $http.post('/Home/Login', { username: vm.username, password: vm.password })
                .then(function (response) {
                    if (response.status === 200) {
                        $window.location.href = '/Home/Devices';
                    } else {
                        vm.errorMessage = 'Invalid username or password';
                    }
                })
                .catch(function (error) {
                    vm.errorMessage = error.data?.Message || 'An error occurred while logging in.';
                })
                .finally(function () {
                    vm.isLoading = false;
                });
        };
    });
