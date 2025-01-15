# Task Management System

### **Created REST API with Relations Using .NET Core**

#### **Objective**:
Developed a compact API for a **Task Management System** focused on **Categories**. In this system, each **Task** is associated with one **Category**, while each **Category** can contain multiple **Tasks**

---

#### **Requirements**:

1. **Environment**:
   - Use .NET Core latest version
   - Configure the project as a **Web API**.

2. **Entities/Models**:
   - Create two models:
     1. **Task**:
        - `Id`
        - `Title` 
        - `Description` 
        - `DueDate`
        - `IsCompleted`
        - `CategoryId`
     2. **Category**:
        - `Id` 
        - `Name`
        - `Description`

3. **Endpoints**:
   - For **Task**:
     - **Create a Task**: `POST /api/tasks`
     - **Get All Tasks**: `GET /api/tasks`
     - **Get a Task by ID**: `GET /api/tasks/{id}`
     - **Update a Task**: `PUT /api/tasks/{id}`
     - **Delete a Task**: `DELETE /api/tasks/{id}`
   - For **Category**:
     - **Create a Category**: `POST /api/categories`
     - **Get All Categories**: `GET /api/categories`
     - **Get a Category by ID**: `GET /api/categories/{id}`
     - **Update a Category**: `PUT /api/categories/{id}`
     - **Delete a Category**: `DELETE /api/categories/{id}`
   - **Get Tasks by Category**: `GET /api/categories/{id}/tasks`
     - Return all tasks belonging to a specific category.

4. **Database**:
   - Use **Entity Framework Core** with **Postgresql** 

5. **Validation**:
   - Ensure the `Title` field in `Task` and the `Name` field in `Category` are required with character limits.

6. **Response Format**:
   - Use **JSON** for request and response payloads.

7. **Testing**:
   - Write **unit tests**. Optional **TDD**

---

#### **Expected Deliverables**:
- A working .NET Core project
- A README file with instructions to:
  - Build and run the application.
  - Test the endpoints using Swagger
- Unit tests, if applicable.

---

### **New Requirements**:

#### 1. **Task Prioritization**
   - Add a `Priority` field to the **Task** entity:
     - Values: `Low`, `Medium`, `High`.
     - Default priority for a new task should be `Medium`.
   - Implement logic to:
     - Sort tasks by priority in `GET /api/tasks`.
     - Allow users to update task priority.

#### 2. **Task Status Management**
   - Introduce a **Task Status Workflow**:
     - Statuses: `Pending`, `In Progress`, `Completed`, `Archived`.
     - Workflow Logic:
       - A task starts with a `Pending` status.
       - Can move to `In Progress` or directly to `Completed`.
       - Once `Completed`, it can be `Archived` (but not modified anymore).
   - Validate status transitions:
     - For example, a task cannot go from `Archived` to any other status.
   - Update the `PUT /api/tasks/{id}` endpoint to handle this workflow.

#### 3. **Category Completion Status**
   - Introduce a `CompletionPercentage` field in **Category**:
     - Calculated as the percentage of tasks marked as `Completed` in that category.
   - Add an endpoint to recalculate and retrieve completion percentages:
     - **`GET /api/categories/{id}/completion`**
       - Returns the `CompletionPercentage` and a summary of tasks in each status.

#### 4. **Reports and Filters**
   - Create a reporting endpoint:
     - **`GET /api/reports/tasks`**
       - Accepts optional query parameters:
         - `status`: Filter tasks by their current status.
         - `priority`: Filter tasks by their priority.
         - `dueBefore`: Return tasks with a `DueDate` earlier than the provided date.
         - `dueAfter`: Return tasks with a `DueDate` later than the provided date.
       - Returns tasks grouped by category.


#### Implemented Deletion Logic
- Different deletion behaviors for tasks based on their priority:
  - Low Priority: Delete directly.
  - Medium Priority: Soft delete (mark the task as inactive or archive it without actually deleting it from the database). 
    - Approach: inserted in another table? - once in deletedTable, can not go away
      - Benefit 1: we do not need to introduce WHERE check in getAll, getById, update, GetTaskStatusStatatisticsForCategoryAsync, DeleteCategoryAsync - easy extensible
      - Benefit 2: less entities - modular
  - High Priority: TBD
    - throw error - saying you need to edit the priority in order to delete it (to medium at max)
    - Approach: Locked State - to be able to continue with it

#### Added Logging
- Example Use Case:
  - Log every operation (e.g., task creation, updates, and deletions) for audit purposes.
  - Specific Log operation for each action

### Applied Design Patterns
- Decorator - used for logging
- Strategy - used for task deletion
- Composite - removed
- Mediator - used for maximum decoupling

### Improved Architecture

- Vertical Slices, MediatR, CQRS

### Structured Namespaces
  - Structured into meaningful segments for easier navigation and to avoid unnecessary content
