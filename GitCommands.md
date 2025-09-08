#### **1. Start a New Feature**
# Ensure you are on the main branch and have the latest version
git checkout master
git pull origin master

# Create a new branch for your feature and switch to it
git checkout -b feature/your-feature-name

---

#### **2. Do Your Work & Commit**
# After making changes to your files, stage them for commit
git add .                          # Stage all changes
# or
git add path/to/specific/file.js   # Stage a specific file

# Create a commit with a descriptive message
git commit -m "Describe the change and why you made it"

---

#### **3. Share Your Work & Get It Reviewed**
# Push your local feature branch to the remote server (GitHub, GitLab)
git push -u origin feature/your-feature-name

---

#### **4. Finalize: Merge and Clean Up**
# 1. Switch your HEAD back to the main branch
git checkout main

# 2. Download the latest changes from the remote (including your merged work!)
git pull origin main

# 3. Delete the feature branch locally (it's no longer needed)
git branch -d feature/your-feature-name

# 4. (Optional) Delete the remote feature branch to keep the remote clean
git push origin --delete feature/your-feature-name

---

#### **Essential Status Commands**
git status      # Shows the state of your working directory and staging area.
git log         # Shows the history of commits.
git diff        # Shows changes not yet staged.
git diff --staged # Shows changes that are staged but not committed.