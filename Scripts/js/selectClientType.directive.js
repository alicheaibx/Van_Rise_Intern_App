angular.module('selectClientType', []).directive('selectClientType', function () {
    return {
        restrict: 'E',
        scope: {
            selectedClient: '=',
        },
        template: `
  <div class="client-select">
    <select class="form-control" 
            ng-model="selectedClient" 
            ng-options="type.id as type.label for type in clientTypes track by type.id">
      <option value="">Select a client type</option> <!-- Keeps the placeholder option -->
    </select>
  </div>
`,
        link: function (scope) {
            // List of client types
            scope.clientTypes = [
                { id: '', label: 'All Types' },
                { id: 1, label: 'Individual' },
                { id: 2, label: 'Organization' }
            ];

            // Watch for changes in selectedClient
            scope.$watch('selectedClient', function (newValue, oldValue) {
                if (newValue !== oldValue) {
                    console.log('Selected Client Type changed from', oldValue, 'to', newValue);
                }
            });
        }
    };
});
