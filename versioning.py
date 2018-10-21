import os.path

class Versioner:
	def __init__(self, filetype, directory, unique_str, replace_str):
		self.filetype = filetype
		self.directory = directory
		self.unique_str = unique_str
		self.replace_str = replace_str

	def replace_file(self, path):
		oldlist = 0;
		with open(path) as f:
			oldlist = f.readlines();
		for i, line in enumerate(oldlist):
			if self.unique_str in line:
				print("Replacing...")
				oldlist[i] = line.replace(self.unique_str, self.replace_str)
		with open(path, 'w') as f:
			f.writelines(oldlist)

	def start(self):
		for dirpath, dirnames, filenames in os.walk(self.directory):
			for filename in filenames:
				current_path = os.path.join(dirpath, filename);
				if filename.endswith(self.filetype):
					print("Versioning: " + current_path)
					self.replace_file(current_path)
					
versioner = Versioner(".cs", os.getcwd(), "$$version$", os.getenv('APPVEYOR_BUILD_VERSION', "XX.XX.XX"))
versioner.start()