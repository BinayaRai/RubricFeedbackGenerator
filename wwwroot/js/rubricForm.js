let criterionIndex = editCriterionCount == null ? 1 : editCriterionCount;
/* ---------- add & remove rows ---------- */
function addCriterion() {
	const tbody = document.getElementById('table-body');
	const index = criterionIndex++;

	const tr = document.createElement('tr');
	tr.id = `criterion-${index}`;
	tr.innerHTML = `
		<td><textarea name="Criteria[${index}].Title" class="form-control" placeholder="Criterion" rows="2"></textarea></td>
		${buildScoreCells(index)}
		<td class="text-center align-middle">
			<button type="button" class="btn btn-danger btn-sm delete-row-btn"
					onclick="removeCriterion(${index})">Delete</button>
		</td>`;
	tbody.appendChild(tr);

	updateRowButtons();
}

function addScoreColumn() {
	const header = document.getElementById('table-header-score');
	const footer = document.querySelector('tfoot tr');
	const rows = document.querySelectorAll('#table-body tr');

	const newIndex = header.children.length - 2; // Skip first and last columns

	// Header cell for score definition
	const th = document.createElement('th');
	th.innerHTML = `
		<div class="input-group">
			<input name="ScoreDefinitions[${newIndex}].ScoreValue" type="number" placeholder="Value" class="form-control text-center fw-bold" style="max-width:50px" />
			<input name="ScoreDefinitions[${newIndex}].ScoreName" placeholder="Score Name" class="form-control fw-bold" />
		</div>
	`;
	header.insertBefore(th, header.lastElementChild);

	// Footer button for deleting column
	const td = document.createElement('td');
	td.className = "text-center align-middle";
	td.innerHTML = `<button type="button" class="btn btn-danger btn-sm delete-col-btn"
						id="delete-col-btn-${newIndex}" onclick="removeScore(${newIndex})">Delete</button>`;
	footer.insertBefore(td, footer.lastElementChild);

	// Add score cells to each existing row
	rows.forEach((row, rowIdx) => {
		const td = document.createElement('td');
		td.innerHTML = `<textarea name="ScoreLevelDescriptions[${rowIdx}][${newIndex}]"
								class="form-control" placeholder="Score Condition" rows="2"></textarea>`;
		row.insertBefore(td, row.lastElementChild);
	});

	reindexScoreColumns();
	updateColButtons();
}


function buildScoreCells(rowIdx) {
	const scoreCols = document.querySelectorAll('#table-header-score th').length - 2;
	let html = '';
	for (let c = 0; c < scoreCols; c++) {
		html += `<td><textarea name="ScoreLevelDescriptions[${rowIdx}][${c}]"
									class="form-control" placeholder="Score Condition" rows="2"></textarea></td>`;
	}
	return html;
}

function removeCriterion(rowIdx) {
	document.getElementById(`criterion-${rowIdx}`)?.remove();
	reindexRows();
	updateRowButtons();
}

function reindexRows() {
	const rows = document.querySelectorAll('#table-body tr');
	rows.forEach((row, r) => {
		row.id = `criterion-${r}`;
		row.querySelector('.delete-row-btn')
			.setAttribute('onclick', `removeCriterion(${r})`);

		// update textarea names in that row
		const tds = row.querySelectorAll('td');
		// first td = criterion title
		tds[0].querySelector('textarea').name = `Criteria[${r}].Title`;
		// following tds = score levels
		for (let c = 1; c < tds.length - 1; c++) {
			const ta = tds[c].querySelector('textarea');
			if (ta) ta.name = `ScoreLevelDescriptions[${r}][${c - 1}]`;
		}
	});
}

function updateRowButtons() {
	const rows = document.querySelectorAll('#table-body tr');
	document.querySelectorAll('.delete-row-btn')
		.forEach(btn => btn.disabled = rows.length === 1);

	document.getElementById('criterion-btn-add').disabled = rows.length >= 10;
}

/* ---------- remove score COLUMN ---------- */
function removeScore(colIdx) {
	const header = document.getElementById('table-header-score');
	const footer = document.querySelector('tfoot tr');
	const rows = document.querySelectorAll('#table-body tr');

	// guard: never drop below 2 score cols
	//const scoreColCount = header.children.length - 2;
	//if (scoreColCount <= 2) return;

	// remove header cell and footer button
	header.children[colIdx + 1]?.remove();
	footer.children[colIdx + 1]?.remove();

	// remove that column's <td> from every data row
	rows.forEach(r => r.children[colIdx + 1]?.remove());

	reindexScoreColumns();
	updateColButtons();
}

function reindexScoreColumns() {
	const headerScore = document.getElementById('table-header-score');
	let colCount = headerScore.children.length;

	// Update colspan for title and description rows
	document.querySelectorAll('.table-header-rubric').forEach(th => {
		th.setAttribute('colspan', colCount);
	});

	// rename header inputs
	for (let c = 1; c < headerScore.children.length - 1; c++) {
		const [val, name] = headerScore.children[c].querySelectorAll('input');
		if (val && name) {
			val.name = `ScoreDefinitions[${c - 1}].ScoreValue`;
			name.name = `ScoreDefinitions[${c - 1}].ScoreName`;
		}
	}
	// rename each row's scorelevel textareas
	const rows = document.querySelectorAll('#table-body tr');
	rows.forEach((row, r) => {
		for (let c = 1; c < headerScore.children.length - 1; c++) {
			const ta = row.children[c].querySelector('textarea');
			if (ta) ta.name = `ScoreLevelDescriptions[${r}][${c - 1}]`;
		}
	});
}

function updateColButtons() {
	const footer = document.querySelector('tfoot tr');
	const btns = footer.querySelectorAll('.delete-col-btn');
	const colCount = document.getElementById('table-header-score').children.length - 2;
	// re-index button ids & onclick
	btns.forEach((b, i) => {
		b.id = `delete-col-btn-${i}`;
		b.setAttribute('onclick', `removeScore(${i})`);
	});

	const scoreCols = document.querySelectorAll('#table-header-score th').length - 2;
	const disable = scoreCols <= 2;
	btns.forEach(b => b.disabled = disable);

	document.getElementById('score-btn-add').disabled = colCount >= 10;
}

/* ---------- initial button state ---------- */
document.addEventListener('DOMContentLoaded', () => {
	updateRowButtons();
	updateColButtons();
    console.log("Rubric form initialized.");
	// Only run initializeFormFromModel if form was not POSTED (no user input exists)
	/*const isEmpty = !document.querySelector('[name="RubricTitle"]')?.value?.trim();
    console.log(`Form is empty: ${isEmpty}`);
	if (isEmpty && window.initialData) {
		initializeFormFromModel();
	}
	criterionIndex = window.initialData.criteria.length;
	console.log(`Added ${criteria.length} criteria rows.`);*/
});

/*function initializeFormFromModel() {
	document.querySelector('[name="RubricTitle"]').value = initialData.rubricTitle;
	document.querySelector('[name="Description"]').value = initialData.rubricDesc;

	// Add score columns based on ScoreDefinitions
	const scoreDefs = initialData.scoreDefinitions || [];
	for (let i = 0; i < scoreDefs.length; i++) {
		if (i >= 5) addScoreColumn(); // default has 5 columns
		document.querySelector(`[name="ScoreDefinitions[${i}].ScoreValue"]`).value = scoreDefs[i].scoreValue;
		document.querySelector(`[name="ScoreDefinitions[${i}].ScoreName"]`).value = scoreDefs[i].scoreName;
	}

	// Add each criterion row
	const criteria = initialData.criteria || [];
	for (let r = 1; r < criteria.length; r++) {
		addCriterion(); // add row dynamically
	}
	
	// Populate Criterion titles
	document.querySelectorAll('#table-body tr').forEach((row, rIdx) => {
		row.querySelector('textarea[name^="Criteria"]').value = criteria[rIdx]?.title || "";
	});

	// Fill in ScoreLevels
	const scoreLevels = initialData.scoreLevels || [];
	scoreLevels.forEach((rowLevels, rIdx) => {
		rowLevels.forEach((sl, cIdx) => {
			const ta = document.querySelector(`textarea[name="ScoreLevelDescriptions[${rIdx}][${cIdx}]"]`);
			if (ta) ta.value = sl.description;
		});
	});
}

/* ---------- task description update ---------- */
document.addEventListener('DOMContentLoaded', function () {
    var select = document.querySelector('select[name="TaskId"]');
    var descriptionBox = document.getElementById('taskDescription');
    var tasks = window.tasks || [];

    if (!select || !descriptionBox) return;

    select.addEventListener('change', function () {
        var selectedId = select.value;
        var task = tasks.find(t => t.TaskId === selectedId);
        descriptionBox.value = task ? task.Description : '';
    });

    // Set initial value if editing
    var initialId = select.value;
    var initialTask = tasks.find(t => t.TaskId === initialId);
    descriptionBox.value = initialTask ? initialTask.Description : '';
});