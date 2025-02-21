angular.module('selectPhones', [])
    .directive('selectPhone', function ($http) {
        return {
            restrict: 'E',
            scope: {
                selectedPhone: '=', // Two-way binding for the selected phone object
                apiUrl: '@'        // Attribute to pass the API URL
            },
            template: `
                <div class="phone-select">
                    <select class="form-control" 
                            ng-model="selectedPhone" 
                            ng-options="phone as phone.Number for phone in phones track by phone.Id">
                        <option value="">Select a phone number</option>
                    </select>
                </div>
            `,
            link: function (scope) {
                scope.phones = [];

                // Fetch all phones from the provided API URL
                if (scope.apiUrl) {
                    $http.get(scope.apiUrl)
                        .then(function (response) {
                            // Bind the API response directly to the scope
                            scope.phones = response.data;
                        })
                        .catch(function (error) {
                            console.error('Error fetching phones:', error);
                            alert('Failed to fetch phone numbers. Please try again.');
                        });
                } else {
                    console.error('API URL is not provided.');
                    alert('API URL is missing. Please provide a valid API URL.');
                }

                // Watch for changes in the selected phone
                scope.$watch('selectedPhone', function (newValue) {
                    console.log('Selected Phone:', newValue);
                });
            }
        };
    });