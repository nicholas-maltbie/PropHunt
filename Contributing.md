# Contributing Guidelines

Before adding anything to the project, make sure to follow the following steps

## 1. Check Out a New Branch

Whenever you are working on a new feature, make sure to checkout a new branch. (where `$branch_name` is the name of the feature you are working on).

```bash
git checkout -b $feature_name
```

Naming convention for branches are as follows. Use a prefix that fits the kind of change the branch is making.

|Type of Branch|Name Format|
|--------------|-----------|
|feature|`feature/$name`|
|hotfix|`hotfix/$name`|
|documentation|`doc/$name`|

## 2. Document Your Code

Whenever you are adding new features or changing the code, make sure to add documentation. Use the [C# XML Documentation Comments](https://marketplace.visualstudio.com/items?itemName=k--kato.docomment) for documentation. This uses XML Documentation for C#. If you have any questions about this code, reference the [doxygen manual](http://www.doxygen.nl/manual/xmlcmds.html).

## 3. ~~Write Unit Test~~

Normally we would recommend writing unit tests for all pieces of code but this as of now is not setup in an easy manner. Ensure that your code works and add it to the environment before committing it. 

Here is a short intro on unity testing [article link](https://www.raywenderlich.com/9454-introduction-to-unity-unit-testing). If we figure out a standardized way to include this we will add it to the project. 

## 4. Make a Pull Request

You cannot push directly to master, you must make a pull request, have it reviewed by other contributors, then commit it to the repository. Only make a PR when the feature is ready to be added or is a significant change in a development feature.

Also, you must ensure you code is up to date. To do this rebase with master (keep commit logs easy to understand). Do this by executing the following commands. 

```bash
# Assuming working on some feature branch named $feature_name

# Go to feature branch
git checkout $feature_name
# Fetch up to date master
git fetch origin master
# Rebase current branch with master
git rebase origin/master
```

See [Creating a pull request](https://help.github.com/en/github/collaborating-with-issues-and-pull-requests/creating-a-pull-request) guide from github for any more questions about how to make pull requests. 

As far as coding style, please try to stay consistent with [Csharp Coding Guidelines](https://wiki.unity3d.com/index.php/Csharp_Coding_Guidelines) from Unity's reference guide. 

## 5. Done

If you follow all these steps, we should have a clean, working master branch.

## 0. Author

Also, add yourself to the authors csv file if you contribute to the repo.
