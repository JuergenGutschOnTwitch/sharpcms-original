// sharpcms is licensed under the open source license GPL - GNU General Public License.

namespace Sharpcms.Data.SiteTree
{
    public class PagePath
    {
        private readonly string _name;
        private readonly string _path;

        public PagePath(string path)
        {
            int lastPathPosition = path.LastIndexOf("/", System.StringComparison.Ordinal);
            if (lastPathPosition > 0)
            {
                _path = path.Substring(0, lastPathPosition);
                _name = path.Substring(lastPathPosition + 1);
            }
            else
            {
                _path = string.Empty;
                _name = path;
            }
        }

        public string Path
        {
            get
            {
                return _path;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }
    }
}