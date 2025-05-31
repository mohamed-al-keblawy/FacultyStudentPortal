fetch('/Faculty/GetStudentScores')
    .then(res => res.json())
    .then(data => {
        const grouped = {};

        data.forEach(row => {
            if (!grouped[row.FullName]) grouped[row.FullName] = [];
            grouped[row.FullName].push({ x: row.AssignmentTitle, y: row.TotalScore });
        });

        const datasets = Object.keys(grouped).map((name, i) => ({
            label: name,
            data: grouped[name],
            borderColor: `hsl(${i * 60}, 70%, 50%)`,
            fill: false
        }));

        new Chart(document.getElementById('lineChart'), {
            type: 'line',
            data: { datasets },
            options: { scales: { x: { title: { display: true, text: 'Assignment' } } } }
        });

        // Optional Pie Chart — just example
        const pieData = data.reduce((acc, row) => {
            acc[row.FullName] = (acc[row.FullName] || 0) + row.TotalScore;
            return acc;
        }, {});

        new Chart(document.getElementById('pieChart'), {
            type: 'pie',
            data: {
                labels: Object.keys(pieData),
                datasets: [{
                    data: Object.values(pieData),
                    backgroundColor: Object.keys(pieData).map((_, i) => `hsl(${i * 60}, 70%, 70%)`)
                }]
            }
        });
    });
