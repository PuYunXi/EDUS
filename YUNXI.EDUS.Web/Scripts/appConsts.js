var app = app || {};
(function () {

    $.extend(app,
        {
            consts: {
                userManagement: {
                    defaultAdminUserName: "admin"
                },
                contentTypes: {
                    formUrlencoded: "application/x-www-form-urlencoded; charset=UTF-8"
                },
                multiTenancySide: {
                    tenant: 1,
                    tenantHost: 2,
                    host: 4
                },
                // 页面查询每次显示条数限制
                maximumNumberofQueries: {
                    machineYield: 5,
                    machineAlarmRecordCount: 20,
                    staffPerformanceRecordCount: 6,
                    dashboardMachineActivationCount: 4,
                    dashboardUsedTimeRateCount: 4,
                    alarmStatisticsCount: 6
                },
                cutterUsedStates: {
                    New: 1,
                    NotLoad: 2,
                    Loading: 3,
                    UnLoad: 4
                },
                // 设备实时状态页面
                machineStates: {
                    pageSize: 9 // 每页最多显示几台设备
                },
                // 综合分析
                yieldAnalysisStatistics: {
                    pageSize: 10 // 最多几台设备的数据一起做比对
                },
                // 实时生产指标->报警信息
                mchineRealtimeAlarms: {
                    pageSize: 12 // 每页最多显示几台设备
                },
                // 固定全局日历的时间范围， 使用场景 => Demo展示时画面好看
                fixedCalendar: {
                    enabled: false,
                    startTime: "2019-8-3",
                    endTime: "2019-8-8"
                }
            }
        });

})();