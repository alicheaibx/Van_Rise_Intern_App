angular.module('selectClients', []).directive('selectClient', function ($http) {
    return {
        restrict: 'E',
        scope: {
            selectedClient: '=', // Two-way binding for the selected client value
            trackBy: '@'        // Attribute to specify which property to track by
        },
        template: `
      <div class="client-select">
        <select class="form-control" 
                ng-model="selectedValue" 
                ng-options="client as client.Name for client in clients track by client.ID">
          <option value="">Select a client</option>
        </select>
      </div>
    `,
        link: function (scope) {
            scope.clients = []; // List of clients fetched from the API
            scope.selectedValue = null; // Intermediate model for the selected value

            // Fetch clients from the API
            $http.get('http://localhost:53211/api/clients/all')
                .then(function (response) {
                    scope.clients = response.data;
                })
                .catch(function (error) {
                    console.error('Error fetching clients:', error);
                });

            // Watch for changes in the selected value
            scope.$watch('selectedValue', function (newValue, oldValue) {
                if (newValue !== oldValue) {
                    scope.selectedClient = newValue; // Update the bound selectedClient
                    console.log('Selected Value:', newValue); // Log the selected value
                }
            });

            // Watch for external changes to selectedClient
            scope.$watch('selectedClient', function (newValue) {
                if (newValue !== scope.selectedValue) {
                    scope.selectedValue = newValue; // Sync the intermediate model
                }
            });
        }
    };
});